using System;
using System.Collections.Generic;
using System.Linq;

namespace NotFluffy.NoFluffDI
{
    public abstract class ResolverFactoryFluent<T, TFactory> :  IResolverFactory
        where TFactory : ResolverFactoryFluent<T, TFactory>
    {
        private readonly Func<IResolutionContext, object> method;
        private bool Transient;
        private readonly List<Type> types = new();
        private List<PostResolveAction> postResolveActions;
        private List<PostDisposeAction> postDisposeActions;
        private object ID { get; set; }

        public ResolverFactoryFluent(Func<IResolutionContext, T> method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            this.method = ctx => method(ctx);
        }

        public IAsyncResolver Create()
        {
            var ids = types.Select(t => new ResolverID(t, ID));
            return Transient
                ? new TransientResolver(ids, method, postResolveActions, postDisposeActions)
                : new SingleResolver(ids, method, postResolveActions, postDisposeActions);
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

        public TFactory As(Type type)
        {
            if(type != null)
                types.Add(type);

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