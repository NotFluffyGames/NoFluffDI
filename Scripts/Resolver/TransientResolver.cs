using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI
{
    public class TransientResolver : BaseTransientResolver, IResolver
    {
        private readonly ResolveMethod method;
        private readonly IReadOnlyList<PostResolveAction> postResolveActions;

        public TransientResolver(
            IEnumerable<ResolverID> IDs, 
            ResolveMethod method, 
            IEnumerable<PostResolveAction> postResolveActions,
            IEnumerable<OnDisposeAction> postDisposeActions)
            : base(IDs, postDisposeActions)
        {
            this.method = method ?? throw new ArgumentNullException(nameof(method));
            this.postResolveActions = postResolveActions?.ToArray();
        }

        public override UniTask<object> ResolveAsync(IResolutionContext context)
        {
            return UniTask.FromResult(Resolve(context));
        }

        public virtual object Resolve(IResolutionContext context)
        {
            var resolved = method(context);
            HandlePostResolveActions(resolved, context);
            
            AddNewResolvedObject(resolved);
            
            IncrementResolveCount();

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