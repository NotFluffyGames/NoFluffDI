using System;
using System.Collections.Generic;
using System.Linq;

namespace NotFluffy.NoFluffDI
{
    public abstract class ResolverFactoryFluent<T> : IResolverFactory
        where T : ResolverFactoryFluent<T>
    {
        private readonly Func<IResolutionContext, object> method;
        private bool Transient;
        private readonly Type methodType;
        private readonly List<Type> types = new();
        protected List<PostResolveAction> postResolveActions;
        protected List<PostDisposeAction> postDisposeActions;
        private object ID { get; set; }

        public ResolverFactoryFluent(Type type, Func<IResolutionContext, object> method, IReadOnlyCollection<Type> extraTypes)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            methodType = type;

            if(extraTypes is { Count: > 0 })
                types.AddRange(extraTypes);

            this.method = method ?? throw new ArgumentNullException(nameof(method));
        }

        public IAsyncResolver Create()
        {
            if(types.Count == 0)
                types.Add(methodType);
            
            var ids = types.Select(t => new ResolverID(t, ID));
            return Transient
                ? new TransientResolver(ids, method, postResolveActions, postDisposeActions)
                : new SingleResolver(ids, method, postResolveActions, postDisposeActions);
        }

        public T AsSingle()
        {
            Transient = false;
            return (T)this;
        }

        public T AsTransient()
        {
            Transient = true;
            return (T)this;
        }
        
        public T WithID(object id)
        {
            ID = id;
            return (T)this;
        }

        public T As<TType>()
        {
            types.Add(typeof(TType));
            return (T)this;
        }

        /// <summary>
        /// Invoked after each new instance is created
        /// </summary>
        public T AddPostResolveAction(PostResolveAction action)
        {
            postResolveActions ??= new List<PostResolveAction>(1);
            postResolveActions.Add(action);

            return (T)this;
        }
        
        public T AddPostDisposeAction(PostDisposeAction action)
        {
            postDisposeActions ??= new List<PostDisposeAction>(1);
            postDisposeActions.Add(action);
            return (T)this;
        }
    }
}