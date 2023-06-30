using System;
using System.Collections.Generic;

namespace NotFluffy.NoFluffDI
{
    public class SingleResolver : TransientResolver
    {
        public SingleResolver(IEnumerable<ResolverID> ids, Func<IResolutionContext, object> method, IEnumerable<PostResolveAction> postResolveActions)
            : base(ids, method, postResolveActions)
        {
        }

        private object instance;

        public override object Resolve(IResolutionContext container)
            => instance ??= base.Resolve(container);
    }
}