using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
// ReSharper disable UnusedMember.Global

namespace NotFluffy.NoFluffDI
{
    public class ResolverFactory : IResolverFactory
    {
        private readonly Func<IResolutionContext, UniTask<object>> method;
        public bool IsLazy { get; private set; } = true;
        private bool Transient;
        private readonly List<Type> types = new();
        private List<PostResolveAction> postResolveActions;
        private object ID { get; set; }

        public ResolverFactory(Type type, Func<IResolutionContext, UniTask<object>> method, IReadOnlyCollection<Type> extraTypes)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            types.Add(type);

            if(extraTypes is { Count: > 0 })
                types.AddRange(extraTypes);

            this.method = method ?? throw new ArgumentNullException(nameof(method));
        }

        public IResolver Create()
        {
            var ids = types.Select(t => new ResolverID(t, ID));
            return Transient
                ? new TransientResolver(ids, method, postResolveActions)
                : new SingleResolver(ids, method, postResolveActions);
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

        public ResolverFactory NonLazy()
        {
            IsLazy = false;
            return this;
        }

        public ResolverFactory Lazy()
        {
            IsLazy = true;
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
    }
}