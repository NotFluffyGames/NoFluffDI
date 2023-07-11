using System;
using System.Collections.Generic;
using System.Linq;

namespace NotFluffy.NoFluffDI
{
    public abstract class BaseResolverFactoryFluent<T, TFactory> : IResolverFactory<T>
        where TFactory : BaseResolverFactoryFluent<T, TFactory>
    {
        protected bool Transient;
        private readonly List<Type> types = new(0);
        private object ID { get; set; }

        private List<OnDisposeAction<T>> onDisposeActions;
        private List<PostResolveAction<T>> postResolveActions;

        public abstract IAsyncResolver Create();

        //All interface methods are converted to fluent methods
        void IResolverFactory.WithID(object id) => WithID(id);
        void IResolverFactory.AsSingle() => AsSingle();
        void IResolverFactory.AsTransient() => AsTransient();
        void IResolverFactory.As<TType>() => As<TType>();
        void IResolverFactory<T>.AddOnDisposeAction(OnDisposeAction<T> action) => AddPostDisposeAction(action);
        void IResolverFactory.AddOnDisposeAction(OnDisposeAction action)
        {
            if (action == null)
                return;
            
            AddPostDisposeAction(disposed => action.Invoke(disposed));
        }
        void IResolverFactory<T>.AddPostResolveAction(PostResolveAction<T> action) => AddPostResolveAction(action);
        void IResolverFactory.AddPostResolveAction(PostResolveAction action)
        {
            if (action == null)
                return;
            
            AddPostResolveAction((resolved, context) => action.Invoke(resolved, context));
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
        
        public TFactory As<TType>()
        {
            types.Add(typeof(TType));
            return (TFactory)this;
        }
        
        public TFactory WithID(object id)
        {
            ID = id;
            return (TFactory)this;
        }
        
        public TFactory AddPostDisposeAction(OnDisposeAction<T> action)
        {
            if(action == null)
                return (TFactory)this;
                
            onDisposeActions ??= new List<OnDisposeAction<T>>(1);
            
            onDisposeActions.Add(action);
            
            return (TFactory)this;
        }
        
        /// <summary>
        /// Invoked after each new instance is created
        /// </summary>
        public TFactory AddPostResolveAction(PostResolveAction<T> action)
        {
            if (action == null)
                return (TFactory)this;

            postResolveActions ??= new List<PostResolveAction<T>>(1);
            
            postResolveActions.Add(action);
            
            return (TFactory)this;
        }

        protected IEnumerable<ResolverID> GetIds()
        {
            if (types.Count == 0)
                yield return new ResolverID(typeof(T), ID);
            else
                foreach (var resolverID in types.Select(t => new ResolverID(t, ID)))
                    yield return resolverID;
        }

        protected IEnumerable<OnDisposeAction> GetOnDisposeActions()
        {
            if(onDisposeActions != null)
                foreach (var onDisposeAction in onDisposeActions)
                    if (onDisposeAction != null)
                        yield return disposed => onDisposeAction((T)disposed);
        }

        protected IEnumerable<PostResolveAction> GetPostResolveActions()
        {
            if(postResolveActions != null)
                foreach (var postResolveAction in postResolveActions)
                    if(postResolveAction != null)
                        yield return (resolved, context) => postResolveAction.Invoke((T)resolved, context);
        }

    }
}