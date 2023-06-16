using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI
{
    public class TransientResolver : IResolver
    {
        private readonly Func<IResolutionContext, UniTask<object>> method;
        private readonly IReadOnlyList<PostResolveAction> postResolveActions;

        public IEnumerable<ResolverID> IDs { get; }
        public int Resolutions { get; private set; }
        
        public TransientResolver(IEnumerable<ResolverID> IDs, Func<IResolutionContext, UniTask<object>> method, IEnumerable<PostResolveAction> postResolveActions)
        {
            this.method = method ?? throw new ArgumentNullException(nameof(method));
            this.IDs = IDs;
            Resolutions = 0;
            this.postResolveActions = postResolveActions?.ToArray();
        }

        public virtual async UniTask<object> Resolve(IResolutionContext context)
        {
            var resolved = await method(context);
            await HandlePostResolveActions(resolved, context);
            ++Resolutions;

            return resolved;
        }

        private UniTask HandlePostResolveActions(object resolved, IResolutionContext context)
        {
            if (resolved == null || postResolveActions == null || postResolveActions.Count == 0)
                return UniTask.CompletedTask;
            
            var tasks = postResolveActions.SelectWhile<PostResolveAction, UniTask>(TryInvoke);
            return UniTask.WhenAll(tasks);

            bool TryInvoke(PostResolveAction action, out UniTask task)
            {
                if (action == null)
                {
                    task = default;
                    return false;
                }

                task = action(resolved, context);
                return true;
            }
        }
    }
}
