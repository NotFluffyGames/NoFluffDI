using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI
{
    public class AsyncTransientResolver : BaseTransientResolver
    {
        private readonly IReadOnlyList<AsyncPostResolveAction> postResolveActions;
        private readonly AsyncResolveMethod method;
        

        public AsyncTransientResolver(
            IEnumerable<ResolverID> IDs, 
            AsyncResolveMethod method, 
            IEnumerable<AsyncPostResolveAction> postResolveActions,
            IEnumerable<PostDisposeAction> postDisposeActions)
            : base(IDs, postDisposeActions)
        {
            this.method = method ?? throw new ArgumentNullException(nameof(method));
            this.postResolveActions = postResolveActions?.ToArray();
        }
        
        public override async UniTask<object> ResolveAsync(IResolutionContext context)
        {
            if (Disposed)
                throw new ObjectDisposedException(GetType().Name);
            
            var resolved = await method(context);
            await HandlePostResolveActions(resolved, context);
            
            AddNewResolvedObject(resolved);

            IncrementResolveCount();

            return resolved;
        }
        
        private UniTask HandlePostResolveActions(object resolved, IResolutionContext context)
        {
            if (postResolveActions == null || postResolveActions.Count == 0)
                return UniTask.CompletedTask;
            
            var tasks = postResolveActions.SelectWhile<AsyncPostResolveAction, UniTask>(TryInvoke);
            return UniTask.WhenAll(tasks);

            bool TryInvoke(AsyncPostResolveAction action, out UniTask task)
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
