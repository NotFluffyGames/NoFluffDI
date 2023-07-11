using System;
using System.Collections.Generic;
using System.Linq;

namespace NotFluffy.NoFluffDI
{
    public abstract class ResolverFactoryFluent<T, TFactory> :  IResolverFactory
        where TFactory : ResolverFactoryFluent<T, TFactory>
    {
        private readonly ResolveMethod<T> method;
        private bool Transient;
        private readonly List<Type> types = new();
        private List<PostResolveAction> postResolveActions;
        private List<PostDisposeAction> postDisposeActions;
        private object ID { get; set; }

        public ResolverFactoryFluent(ResolveMethod<T> method)
        {
            this.method = method ?? throw new ArgumentNullException(nameof(method));
        }

        public IAsyncResolver Create()
        {
            if(types.Count == 0)
                types.Add(typeof(T));
            
            var ids = types.Select(t => new ResolverID(t, ID));

            return Transient
                ? new TransientResolver(ids, ResolveMethod, postResolveActions, postDisposeActions)
                : new SingleResolver(ids, ResolveMethod, postResolveActions, postDisposeActions);
            
            object ResolveMethod(IResolutionContext ctx) => method(ctx);
        }

        public TFactory AsSingle()
        {
            Transient = false;
            return (TFactory)this;
        }

        public TFactory AsTransient()
        {
            Transient = true;
            return (TFactory)this;
        }
        
        public TFactory WithID(object id)
        {
            ID = id;
            return (TFactory)this;
        }

        public TFactory As<TType>()
        {
            types.Add(typeof(TType));

            return (TFactory)this;
        }

        /// <summary>
        /// Invoked after each new instance is created
        /// </summary>
        public TFactory AddPostResolveAction(PostResolveAction<T> action)
        {
            postResolveActions ??= new List<PostResolveAction>(1);
            postResolveActions.Add((r, c) => action((T)r,c));

            return (TFactory)this;
        }
        
        public TFactory AddPostDisposeAction(PostDisposeAction<T> action)
        {
            postDisposeActions ??= new List<PostDisposeAction>(1);
            postDisposeActions.Add(d => action((T)d));
            return (TFactory)this;
        }
    }
}