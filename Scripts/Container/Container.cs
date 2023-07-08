using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Cysharp.Threading.Tasks;
using NotFluffy.NoFluffRx;

namespace NotFluffy.NoFluffDI
{
    public class ContainerBuilder : BaseContainerBuilder
    {
        private event Action<IReadOnlyContainer> _onInjectionComplete;
        
        public ContainerBuilder(object context = null, IReadOnlyContainer parent = null)
            : base(context, parent)
        {
        }

        protected override IContainerBuildResult Create()
        {
            AssertNotDisposed();
                
            var injectionCompletionSource = new UniTaskCompletionSource();

            var container = new Container(
                Context,
                Parent,
                resolverFactories,
                () => injectionCompletionSource.Task);

            var injectContext = new InjectContext(container);
            
            if(injectables is { Count: > 0 })
                _ = UniTask.WhenAll(injectables.SelectWhile<Inject, UniTask>(TryInject)).ContinueWith(OnComplete);
            else
                OnComplete();

            return new ContainerBuildResult(container, container);

            bool TryInject(Inject inject, out UniTask task)
            {
                if (inject == null)
                {
                    task = default;
                    return false;
                }

                task = inject(injectContext);
                return true;
            }

            void OnComplete()
            {
                injectionCompletionSource.TrySetResult();
                
                injectContext.InjectionComplete();
                
                _onInjectionComplete?.Invoke(container);
            }
        }

        public override IContainerBuilder RegisterInjectCallback(Action<IReadOnlyContainer> callback)
        {
            AssertNotDisposed();
            
            _onInjectionComplete += callback;

            return this;
        }


        private class Container : IReadOnlyContainer, IReactiveDisposable
        {
            public object Context { get; }

            public IReadOnlyDictionary<ResolverID, IAsyncResolver> Resolvers { get; }

            private bool disposed;

            private readonly HashSet<ResolverID> _currentlyResolved = new();
            private readonly Func<UniTask> injectionTaskSource;
            public UniTask InjectionTask => injectionTaskSource();

            private readonly Subject<Unit> onDispose = new();
            public IObservable<Unit> OnDispose => onDispose;

            public Container(
                object context,
                IReadOnlyContainer parent,
                IEnumerable<IResolverFactory> factories,
                Func<UniTask> injectionTask)
            {
                Context = context;
                injectionTaskSource = injectionTask;
                
                var resolversDict = parent == null 
                    ? new Dictionary<ResolverID, IAsyncResolver>()
                    : new Dictionary<ResolverID, IAsyncResolver>(parent.Resolvers);
                
                Resolvers = resolversDict;

                if(factories != null)
                    foreach (var resolver in BuildResolvers(factories))
                        if(resolver is { IDs: not null })
                            foreach (var id in resolver.IDs)
                                resolversDict[id] = resolver;

                var hashset = new HashSet<IAsyncResolver>();

                foreach (var resolver in resolversDict.Values)
                    hashset.Add(resolver);

                //Disposed on container dispose
                var composite = new CompositeDisposable(hashset.Count).AddTo(this);
                foreach (var resolver in hashset)
                    resolver.TakeRefCountToken().AddTo(composite);
            }

            public void Dispose()
            {
                if (disposed)
                    return;

                disposed = true;
                
                onDispose.OnNext();
            }

            public IContainerBuilder Scope(object context)
            {
                return new ContainerBuilder(context, this);
            }

            public override string ToString() => Context.ToString();

            public bool Contains(Type contract, object id = null)
            {
                var typesToCheck = new Queue<Type>();
                typesToCheck.Enqueue(contract);

                while (typesToCheck.Count > 0)
                {
                    var type = typesToCheck.Dequeue();

                    var resolverID = new ResolverID(type, id);
                    //First look for a direct resolver
                    if (GetResolver(resolverID) != null)
                        return true;
                }

                return false;
            }

            private IEnumerable<IAsyncResolver> BuildResolvers(IEnumerable<IResolverFactory> factories)
            {
                if (factories == null)
                    yield break;

                var toResolve = new Queue<IAsyncResolver>();
                foreach (var factory in factories)
                {
                    var resolver = factory?.Create();

                    if (resolver == null)
                        continue;
                    
                    yield return resolver;

                    if (!factory.IsLazy)
                        toResolve.Enqueue(resolver);
                }

                while (toResolve.Count > 0)
                {
                    var resolver = toResolve.Dequeue();
                    var ctx = new ResolutionContext(this);
                    resolver.ResolveAsync(ctx);
                }
            }
            
            public object Resolve(Type contract, object id = null)
            {
                var resolverID = new ResolverID(contract, id);
                
                var asyncResolver = GetResolver(resolverID);

                if (asyncResolver is not IResolver resolver)
                    throw new NoMatchingResolverException(contract);
                
                if (!_currentlyResolved.Add(resolverID))
                    throw new CircularDependencyException(resolverID);
                
                var value = resolver.Resolve(new ResolutionContext(this)); 

                _currentlyResolved.Remove(resolverID);
                
                return value;
            }
            
            public async UniTask<object> ResolveAsync(Type contract, object id = null)
            {
                var resolverID = new ResolverID(contract, id);
                
                var resolver = GetResolver(resolverID);

                if (resolver == null)
                    throw new NoMatchingResolverException(contract);

                if (!_currentlyResolved.Add(resolverID))
                    throw new CircularDependencyException(resolverID);
                
                var value = await resolver.ResolveAsync(new ResolutionContext(this)); 

                _currentlyResolved.Remove(resolverID);
                
                return value;
            }

            private IAsyncResolver GetResolver(ResolverID resolverId) 
                => Resolvers.TryGetValue(resolverId, out var r) ? r : null;

            ~Container()
            {
                Dispose();
            }
        }
        
        private class InjectContext : IInjectContext
        {
            public IReadOnlyContainer Container { get; }

            private event Action OnComplete;

            internal void InjectionComplete()
            {
                OnComplete?.Invoke();
                OnComplete = null;
            }
        
            public void RegisterInjectCallback(Action callback)
            {
                OnComplete += callback;
            }

            public InjectContext(IReadOnlyContainer container)
            {
                Container = container;
            }
        }
    }
}