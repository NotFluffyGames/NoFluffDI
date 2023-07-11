using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI
{
    public abstract class AsyncResolverFactoryFluent<T, TFactory> : BaseResolverFactoryFluent<T, TFactory>, IAsyncResolverFactory<T>
        where TFactory : AsyncResolverFactoryFluent<T, TFactory>
    {
        private readonly AsyncResolveMethod<T> method;
        private List<AsyncPostResolveAction<T>> postResolveActions;

        protected AsyncResolverFactoryFluent(AsyncResolveMethod<T> method)
        {
            this.method = method ?? throw new ArgumentNullException(nameof(method));
        }

        public override IAsyncResolver Create()
        {
            return Transient
                    ? new AsyncTransientResolver(GetIds(), Resolve, GetAsyncPostResolveActions(), GetOnDisposeActions())
                    : new AsyncSingleResolver(GetIds(), Resolve, GetAsyncPostResolveActions(), GetOnDisposeActions());

            async UniTask<object> Resolve(IResolutionContext context) => await method(context);
        }

        //All interface methods are converted to fluent methods
        void IAsyncResolverFactory<T>.AddAsyncPostResolveAction(AsyncPostResolveAction<T> action) => AddAsyncPostResolveAction(action);
        void IAsyncResolverFactory.AddAsyncPostResolveAction(AsyncPostResolveAction action)
        {
            if (action == null)
                return;
            
            AddAsyncPostResolveAction((resolved, context) => action.Invoke(resolved, context));
        }
        
        public TFactory AddAsyncPostResolveAction(AsyncPostResolveAction<T> action)
        {
            postResolveActions ??= new List<AsyncPostResolveAction<T>>(1);
            
            if(action != null)
                postResolveActions.Add(action);
            
            return (TFactory)this;
        }

        private IEnumerable<AsyncPostResolveAction> GetAsyncPostResolveActions()
        {
            if(postResolveActions != null)
                foreach (var postResolveAction in postResolveActions)
                    if (postResolveAction != null)
                        yield return (resolved, context) => postResolveAction((T)resolved, context);
            
            foreach (var postResolveAction in base.GetPostResolveActions())
                if (postResolveAction != null)
                {
                    yield return PostResolveAction;
                    
                    UniTask PostResolveAction(object resolved, IResolutionContext context)
                    {
                        postResolveAction((T)resolved, context);
                        return UniTask.CompletedTask;
                    }
                }
        }
    }
}