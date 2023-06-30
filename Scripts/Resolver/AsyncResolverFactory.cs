using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
// ReSharper disable UnusedMember.Global

namespace NotFluffy.NoFluffDI
{
    public class AsyncResolverFactory : IResolverFactory
    {
        private readonly Func<IResolutionContext, UniTask<object>> method;
        public bool IsLazy { get; private set; } = true;
        private bool Transient;
        private readonly List<Type> types = new();
        private List<AsyncPostResolveAction> postResolveActions;
        private object ID { get; set; }

        public AsyncResolverFactory(Type type, Func<IResolutionContext, UniTask<object>> method, IReadOnlyCollection<Type> extraTypes)
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
                ? new AsyncTransientResolver(ids, method, postResolveActions)
                : new AsyncSingleResolver(ids, method, postResolveActions);
        }

        public AsyncResolverFactory AsSingle()
        {
            Transient = false;
            return this;
        }

        public AsyncResolverFactory AsTransient()
        {
            Transient = true;
            return this;
        }

        public AsyncResolverFactory NonLazy()
        {
            IsLazy = false;
            return this;
        }

        public AsyncResolverFactory Lazy()
        {
            IsLazy = true;
            return this;
        }
        public AsyncResolverFactory WithID(object id)
        {
            ID = id;
            return this;
        }

        /// <summary>
        /// Invoked after each new instance is created
        /// </summary>
        public AsyncResolverFactory AddPostResolveAction(AsyncPostResolveAction action)
        {
            postResolveActions ??= new List<AsyncPostResolveAction>();
            postResolveActions.Add(action);

            return this;
        }
    }
}