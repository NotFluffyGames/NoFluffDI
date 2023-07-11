using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI
{
    public abstract class BaseTransientResolver : IAsyncResolver
    {
        private readonly IReadOnlyList<OnDisposeAction> postDisposeActions;
        private readonly bool hasPostDisposeActions;

        public IReadOnlyList<ResolverID> IDs { get; }

        protected virtual int ResolvedObjectsCapacity => 4;

        private List<object> resolvedObjects;
        protected bool Disposed { get; private set; }
        private int references;
        public int Resolutions { get; private set; }

        protected BaseTransientResolver(
            IEnumerable<ResolverID> IDs,
            IEnumerable<OnDisposeAction> postDisposeActions)
        {
            Resolutions = 0;
            this.IDs = IDs.ToArray();
            this.postDisposeActions = postDisposeActions?.ToArray();

            hasPostDisposeActions = this.postDisposeActions?.Any() ?? false;
        }

        public abstract UniTask<object> ResolveAsync(IResolutionContext context);

        public IDisposable TakeRefCountToken()
        {
            if(Disposed)
                return Disposable.Empty;
            
            references++;
            return Disposable.Create(OnDispose);
            
            void OnDispose()
            {
                references--;
                if(references <= 0)
                    Dispose();
            }
        }

        public virtual void Dispose()
        {
            if(Disposed)
                return;
            
            Disposed = true;
            GC.SuppressFinalize(this);
            
            if(hasPostDisposeActions)
                foreach (var postDisposeAction in postDisposeActions)
                foreach (var resolvedObject in resolvedObjects)
                    postDisposeAction?.Invoke(resolvedObject);
        }
        
        ~BaseTransientResolver()
        {
            Dispose();   
        }

        protected void AddNewResolvedObject(object resolvedObject)
        {
            if(hasPostDisposeActions)
                resolvedObjects.Add(resolvedObject);
        }

        protected void IncrementResolveCount() => Resolutions++;
    }
}