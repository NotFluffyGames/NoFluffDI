using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI
{
    public class ResolverFactory : IResolverFactory
    {
        private readonly Func<IResolutionContext, UniTask<object>> method;
        public bool IsLazy { get; private set; } = true;
        private bool Transient;
        private readonly List<Type> types = new();
        private object ID { get; set; }

        public ResolverFactory(Type type, Func<IResolutionContext, UniTask<object>> method, params Type[] extraTypes)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            types.Add(type);

            if(extraTypes is { Length: > 0 })
                types.AddRange(extraTypes);

            this.method = method ?? throw new ArgumentNullException(nameof(method));
        }

        public IResolver Create()
        {
            var ids = types.Select(t => new ResolverID(t, ID));
            if (Transient)
                return new TransientResolver(ids, method);
            else
                return new SingleResolver(ids, method);
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
    }
}