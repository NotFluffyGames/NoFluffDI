using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI
{
    public abstract class AsyncResolverFactoryFluent<T, TFactory> : IResolverFactory
        where TFactory : AsyncResolverFactoryFluent<T, TFactory>
    {
        private readonly AsyncResolveMethod<T> method;
        private bool Transient;
        private readonly List<Type> types = new(0);
        private List<AsyncPostResolveAction> postResolveActions;
        private List<PostDisposeAction> postDisposeActions;
        private object ID { get; set; }

        protected AsyncResolverFactoryFluent(AsyncResolveMethod<T> method)
        {
            this.method = method ?? throw new ArgumentNullException(nameof(method));
        }

        public IAsyncResolver Create()
        {
                IEnumerable<ResolverID> ids;
                
                if (types.Count == 0)
                    ids = Enumerable.Repeat(new ResolverID(typeof(T), ID), 1);
                else
                    ids = types.Select(t => new ResolverID(t, ID));

                return Transient
                    ? new AsyncTransientResolver(ids, Resolve, postResolveActions, postDisposeActions)
                    : new AsyncSingleResolver(ids, Resolve, postResolveActions, postDisposeActions);

                async UniTask<object> Resolve(IResolutionContext context) => await method(context);
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
        public TFactory AddPostResolveAction(AsyncPostResolveAction action)
        {
            postResolveActions ??= new List<AsyncPostResolveAction>(1) { action };
            return (TFactory)this;
        }

        public TFactory AddPostDisposeAction(PostDisposeAction action)
        {
            postDisposeActions ??= new List<PostDisposeAction>(1) { action };
            return (TFactory)this;
        }
    }
}