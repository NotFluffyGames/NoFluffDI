using System;
using System.Collections.Generic;

namespace NotFluffy.NoFluffDI
{
    public abstract class BaseContainerBuilder : IContainerBuilder
    {
        private bool _disposed;
        
        protected List<IResolverFactory> resolverFactories;
        protected object Context { get; }
        protected IReadOnlyContainer Parent { get; }
        protected event Action<IReadOnlyContainer> BuildCallback;
        protected List<Inject> injectables;

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
            
            var container = Create(); 
            
            BuildCallback?.Invoke(container.Container);
            BuildCallback = null;
            
            return container;
        }

        protected abstract IContainerBuildResult Create();

        public IContainerBuilder RegisterBuildCallback(Action<IReadOnlyContainer> callback)
        {
            AssertNotDisposed();
            
            BuildCallback += callback;

            return this;
        }

        public abstract IContainerBuilder RegisterInjectCallback(Action<IReadOnlyContainer> callback);

        public virtual void Dispose()
        {
            if(_disposed)
                return;
            
            _disposed = true;
            
            resolverFactories = null;
            BuildCallback = null;
            injectables = null;
        }
        
        protected void AssertNotDisposed()
        {
            if(_disposed)
                throw new ObjectDisposedException(nameof(ContainerBuilder));
        }
    }
}