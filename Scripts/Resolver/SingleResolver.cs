using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI
{
    public class SingleResolver : TransientResolver
    {
        public SingleResolver(IEnumerable<ResolverID> ids, Func<IResolutionContext, UniTask<object>> method)
            : base(ids, method)
        {

        }

        private object instance;

        public override async UniTask<object> Resolve(IResolutionContext container)
            => instance ??= await base.Resolve(container);
    }
}