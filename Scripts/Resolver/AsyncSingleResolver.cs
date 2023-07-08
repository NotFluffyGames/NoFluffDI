using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI
{
    public class AsyncSingleResolver : AsyncTransientResolver
    {
        public AsyncSingleResolver(
            IEnumerable<ResolverID> ids, 
            Func<IResolutionContext, UniTask<object>> method, 
            IEnumerable<AsyncPostResolveAction> postResolveActions, 
            IEnumerable<PostDisposeAction> postDisposeActions)
            : base(ids, method, postResolveActions, postDisposeActions)
        {
        }

        private object instance;

        public override async UniTask<object> ResolveAsync(IResolutionContext container)
        {
            if (instance == null)
                return instance = await base.ResolveAsync(container);

            IncrementResolveCount();
            return instance;
        }

        protected override int ResolvedObjectsCapacity => 1;
    }
}