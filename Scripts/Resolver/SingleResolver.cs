using System;
using System.Collections.Generic;

namespace NotFluffy.NoFluffDI
{
    public class SingleResolver : TransientResolver
    {
        public SingleResolver(IEnumerable<ResolverID> ids, Func<IResolutionContext, object> method)
            : base(ids, method)
        {

        }

        private object instance;

        public override object Resolve(IResolutionContext container)
            => instance ??= base.Resolve(container);
    }
}