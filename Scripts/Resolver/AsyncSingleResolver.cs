using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI
{
    public class AsyncSingleResolver : AsyncTransientResolver
    {
        public AsyncSingleResolver(IEnumerable<ResolverID> ids, Func<IResolutionContext, UniTask<object>> method, IEnumerable<AsyncPostResolveAction> postResolveActions)
            : base(ids, method, postResolveActions)
        {
        }

        private object instance;

        public override async UniTask<object> ResolveAsync(IResolutionContext container)
            => instance ??= await base.ResolveAsync(container);
    }
}