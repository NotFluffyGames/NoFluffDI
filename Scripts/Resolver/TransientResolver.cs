using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI
{
    public class TransientResolver : IResolver
    {
        private readonly Func<IResolutionContext, UniTask<object>> method;
        
        public IEnumerable<ResolverID> IDs { get; }
        public int Resolutions { get; private set; }
        
        public TransientResolver(IEnumerable<ResolverID> IDs, Func<IResolutionContext, UniTask<object>> method)
        {
            this.method = method ?? throw new ArgumentNullException(nameof(method));
            this.IDs = IDs;
            Resolutions = 0;
        }

        public virtual UniTask<object> Resolve(IResolutionContext container)
        {
            ++Resolutions;
            return method(container);
        }
    }
}
