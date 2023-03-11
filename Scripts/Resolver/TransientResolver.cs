using System;
using System.Collections.Generic;

namespace NotFluffy.NoFluffDI
{
    public class TransientResolver : IResolver
    {
        public TransientResolver(IEnumerable<ResolverID> IDs, Func<IResolutionContext, object> method)
        {
            this.method = method ?? throw new ArgumentNullException(nameof(method));
            this.IDs = IDs;
            Resolutions = 0;
        }

        public IEnumerable<ResolverID> IDs { get; }
        public int Resolutions { get; private set; }


        private readonly Func<IResolutionContext, object> method;

        public virtual object Resolve(IResolutionContext container)
        {
            ++Resolutions;
            return method(container);
        }
    }
}
