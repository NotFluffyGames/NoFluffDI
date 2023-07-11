using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI
{
    public abstract class BaseContainerBuilder : IContainerBuilder
    {
        private bool _disposed;
        
        protected List<IResolverFactory> resolverFactories;
        protected object Context { get; }
        protected IReadOnlyContainer Parent { get; }
        protected List<Inject> injectables;
        private event BuildCallback BuildCallback;
        private event InjectCallback InjectCallback;

        protected BaseContainerBuilder(object context = null, IReadOnlyContainer parent = null)
        {
            Parent = parent;
            Context = context;
        }

        public IContainerBuilder Add(IResolverFactory resolverFactory)
        {
            AssertNotDisposed();
            
            resolverFactories ??= new List<IResolverFactory>();
            resolverFactories.Add(resolverFactory);
            return this;
        }

        public IContainerBuilder AddInjectable(Inject injectable)
        {
            AssertNotDisposed();
            
            injectables ??= new List<Inject>();
            
            injectables.Add(injectable);
            
            return this;
        }

        public IContainerBuildResult Build()
        {
            AssertNotDisposed();
            
            var result = Create();
            var container = result.Container;
            
            BuildCallback?.Invoke(container);
            var injectCallback = InjectCallback;

            _ = new InjectContext(container)
                .Inject(injectables)
                .ContinueWith(InjectionComplete);
            
            Dispose();

            return result;

            void InjectionComplete()
            {
                injectCallback?.Invoke(container);
            }
        }

        protected abstract IContainerBuildResult Create();

        public IContainerBuilder RegisterBuildCallback(BuildCallback callback)
        {
            AssertNotDisposed();
            
            BuildCallback += callback;

            return this;
        }

        public IContainerBuilder RegisterInjectCallback(InjectCallback callback)
        {
            AssertNotDisposed();
            
            InjectCallback += callback;

            return this;
        }

        public virtual void Dispose()
        {
            if(_disposed)
                return;
            
            _disposed = true;
            
            resolverFactories = null;
            BuildCallback = null;
            InjectCallback = null;
            injectables = null;
        }
        protected void AssertNotDisposed()
        {
            if(_disposed)
                throw new ObjectDisposedException(nameof(ContainerBuilder));
        }
    }
}