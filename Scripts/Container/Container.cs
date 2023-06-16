using System;
using System.Collections.Generic;
using System.Linq;
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

            public IReadOnlyDictionary<ResolverID, IResolver> Resolvers { get; }

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
                    //First look for a direct resolver
                    if (GetResolver(type, id) != null)
                        return true;
                }

                return false;
            }


            private IReadOnlyDictionary<ResolverID, IResolver> BuildResolvers(IEnumerable<IResolverFactory> factories)
            {
                if (factories == null)
                    return null;

                var resolvers = new Dictionary<ResolverID, IResolver>();

                var toResolve = new Queue<IResolver>();
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
                    var ctx = new ResolutionContext(resolver, this, this);
                    resolver.Resolve(ctx);
                }

                return resolvers;
            }

            public async UniTask<object> Resolve(Type contract, object id = null)
            {
                var resolverID = new ResolverID(contract, id);
                
                //Try resolve using a direct resolver
                var ctx = GetResolver(resolverID);

                if (ctx == null)
                    throw new NoMatchingResolverException(contract);
                
                return await ctx.Resolve();
            }

            private IResolutionContext GetResolver(Type contract, object id = null)
                => GetResolver(new ResolverID(contract, id));

            private IResolutionContext GetResolver(ResolverID resolverId)
            {
                //Try resolve using a direct resolver
                return SelfAndParents()
                    .SelectWhile<IReadOnlyContainer, IResolutionContext>(TryGetResolver)
                    .FirstOrDefault();

                bool TryGetResolver(IReadOnlyContainer node, out IResolutionContext c)
                {
                    if (node.Resolvers == null)
                    {
                        c = default;
                        return false;
                    }

                    if (node.Resolvers.TryGetValue(resolverId, out var r))
                    {
                        c = new ResolutionContext(r, node, this);
                        return true;
                    }

                    c = default;
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