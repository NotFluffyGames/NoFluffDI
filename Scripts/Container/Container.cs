using System;
using System.Collections.Generic;
using System.Reactive;
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
                resolvers,
                () => injectionCompletionSource.Task,
                Parent);

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
            public IReadOnlyContainer Parent { get; private set; }

            public object Context { get; }

            public IReadOnlyDictionary<ResolverID, IAsyncResolver> Resolvers { get; }

            private bool disposed;

            private readonly Func<UniTask> injectionTaskSource;
            public UniTask InjectionTask => injectionTaskSource();

            private readonly Subject<Unit> onDispose = new();
            public IObservable<Unit> OnDispose => onDispose;

            public Container(
                object context,
                IEnumerable<IResolverFactory> resolvers,
                Func<UniTask> injectionTask,
                IReadOnlyContainer parent = null)
            {
                Context = context;
                injectionTaskSource = injectionTask;
                Parent = parent;

                Parent?.OnDispose.Subscribe(Dispose);

                Resolvers = BuildResolvers(resolvers);
            }

            public void Dispose()
            {
                if (disposed)
                    return;

                disposed = true;

                Parent = null;
                onDispose.OnNext(new Unit());
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
                    if (GetResolver(resolverID, out _) != null)
                        return true;
                }

                return false;
            }

            private IReadOnlyDictionary<ResolverID, IAsyncResolver> BuildResolvers(IEnumerable<IResolverFactory> factories)
            {
                if (factories == null)
                    return null;
                
                var resolvers = new Dictionary<ResolverID, IAsyncResolver>();

                var toResolve = new Queue<IAsyncResolver>();
                foreach (var factory in factories)
                {
                    var resolver = factory.Create();

                    foreach (var id in resolver.IDs)
                        resolvers[id] = resolver;

                    if (!factory.IsLazy)
                        toResolve.Enqueue(resolver);
                }

                while (toResolve.Count > 0)
                {
                    var resolver = toResolve.Dequeue();
                    var ctx = new ResolutionContext(this, this);
                    resolver.ResolveAsync(ctx);
                }

                return resolvers;
            }
            
            public object Resolve(Type contract, object id = null)
            {
                var resolverID = new ResolverID(contract, id);
                
                var asyncResolver = GetResolver(resolverID, out var sourceContainer);

                if (asyncResolver is not IResolver resolver)
                    throw new NoMatchingResolverException(contract);
                
                return resolver.Resolve(new ResolutionContext(this, sourceContainer));
            }
            
            public async UniTask<object> ResolveAsync(Type contract, object id = null)
            {
                var resolverID = new ResolverID(contract, id);
                
                var resolver = GetResolver(resolverID, out var sourceContainer);

                if (resolver == null)
                    throw new NoMatchingResolverException(contract);
                
                return await resolver.ResolveAsync(new ResolutionContext(this, sourceContainer));
            }

            private IAsyncResolver GetResolver(ResolverID resolverId, out IReadOnlyContainer sourceContainer)
            {
                //Try resolve using a direct resolver
                foreach (var container in SelfAndParents())
                {
                    if (TryGetResolver(container, out var resolver))
                    {
                        sourceContainer = container;
                        return resolver;
                    }
                }

                sourceContainer = null;
                return null;

                bool TryGetResolver(IReadOnlyContainer node, out IAsyncResolver resolver)
                {
                    if (node.Resolvers == null)
                    {
                        resolver = default;
                        return false;
                    }

                    if (node.Resolvers.TryGetValue(resolverId, out var r))
                    {
                        resolver = r;
                        return true;
                    }

                    resolver = default;
                    return false;
                }
            }

            private IEnumerable<IReadOnlyContainer> SelfAndParents()
            {
                IReadOnlyContainer current = this;

                while (current != null)
                {
                    yield return current;
                    current = current.Parent;
                }
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