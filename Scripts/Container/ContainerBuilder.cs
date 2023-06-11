using System;
using System.Collections.Generic;

namespace NotFluffy.NoFluffDI
{
    public abstract class BaseContainerBuilder : IContainerBuilder, IDisposable
    {
        private bool _disposed;
        
        protected List<IResolverFactory> resolvers;
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
            
            resolvers ??= new List<IResolverFactory>();
            resolvers.Add(resolverFactory);
            return this;
        }

        public IContainerBuilder AddInjectable(Inject injectable)
        {
            AssertNotDisposed();
            
            injectables ??= new List<Inject>();
            return this;
        }

        public IReadOnlyContainer Build()
        {
            AssertNotDisposed();
            
            var container = Create(); 
            
            BuildCallback?.Invoke(container);
            BuildCallback = null;
            
            return container;
        }

        protected abstract IReadOnlyContainer Create();

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
            
            resolvers = null;
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