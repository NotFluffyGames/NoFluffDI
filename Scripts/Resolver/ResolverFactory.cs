using System;
using System.Collections.Generic;
using System.Linq;

namespace NotFluffy.NoFluffDI
{
    public class ResolverFactory : IResolverFactory
    {
        private readonly Func<IResolutionContext, object> method;
        private bool Transient;
        private readonly List<Type> types = new();
        private List<PostResolveAction> postResolveActions;
        private List<PostDisposeAction> postDisposeActions;
        private object ID { get; set; }

        public ResolverFactory(Type type, Func<IResolutionContext, object> method, IReadOnlyCollection<Type> extraTypes)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            types.Add(type);

            if(extraTypes is { Count: > 0 })
                types.AddRange(extraTypes);

            this.method = method ?? throw new ArgumentNullException(nameof(method));
        }

        public IAsyncResolver Create()
        {
            var ids = types.Select(t => new ResolverID(t, ID));
            return Transient
                ? new TransientResolver(ids, method, postResolveActions, postDisposeActions)
                : new SingleResolver(ids, method, postResolveActions, postDisposeActions);
        }

        public ResolverFactory AsSingle()
        {
            Transient = false;
            return this;
        }

        public ResolverFactory AsTransient()
        {
            Transient = true;
            return this;
        }
        
        public ResolverFactory WithID(object id)
        {
            ID = id;
            return this;
        }

        /// <summary>
        /// Invoked after each new instance is created
        /// </summary>
        public ResolverFactory AddPostResolveAction(PostResolveAction action)
        {
            postResolveActions ??= new List<PostResolveAction>();
            postResolveActions.Add(action);

            return this;
        }
        
        public ResolverFactory AddPostDisposeAction(PostDisposeAction action)
        {
            postDisposeActions ??= new List<PostDisposeAction>(1) { action };
            return this;
        }
    }
}