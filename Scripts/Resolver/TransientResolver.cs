using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI
{
    public class TransientResolver : IResolver
    {
        private readonly Func<IResolutionContext, object> method;
        private readonly IReadOnlyList<PostResolveAction> postResolveActions;

        public IEnumerable<ResolverID> IDs { get; }
        public int Resolutions { get; private set; }
        
        public TransientResolver(IEnumerable<ResolverID> IDs, Func<IResolutionContext, object> method, IEnumerable<PostResolveAction> postResolveActions)
        {
            this.method = method ?? throw new ArgumentNullException(nameof(method));
            this.IDs = IDs;
            Resolutions = 0;
            this.postResolveActions = postResolveActions?.ToArray();
        }

        public UniTask<object> ResolveAsync(IResolutionContext context)
        {
            return UniTask.FromResult(Resolve(context));
        }

        public virtual object Resolve(IResolutionContext context)
        {
            var resolved = method(context);
            HandlePostResolveActions(resolved, context);
            ++Resolutions;

            return resolved;
        }

        private void HandlePostResolveActions(object resolved, IResolutionContext context)
        {
            if (postResolveActions == null || postResolveActions.Count == 0)
                return;

            foreach (var action in postResolveActions)
                action?.Invoke(resolved, context);
        }
    }
}