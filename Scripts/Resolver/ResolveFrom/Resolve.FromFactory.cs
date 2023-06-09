using System;
using Cysharp.Threading.Tasks;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace NotFluffy.NoFluffDI
{
    public static partial class Resolve
    {
        public static ResolverFactory FromFactory<TOut, TIn>(IFactory<TOut, TIn> factory)
        {
            return FromMethodAsync(Create);

            async UniTask<TOut> Create(IResolutionContext resolver) => factory.Create(await resolver.Resolve<TIn>());
        }

        public static ResolverFactory FromFactory<TOut, TIn>(IFactory<TOut, TIn> factory, params Type[] extraTypes)
        {
            return FromMethodAsync(Create, extraTypes);

            async UniTask<TOut> Create(IResolutionContext resolver) => factory.Create(await resolver.Resolve<TIn>());
        }

        public static ResolverFactory FromFactoryAsync<TOut, TIn>(IFactoryAsync<TOut, TIn> factory)
        {
            return FromMethodAsync(Create);

            async UniTask<TOut> Create(IResolutionContext resolver) => await factory.Create(await resolver.Resolve<TIn>());
        }

        public static ResolverFactory FromFactoryAsync<TOut, TIn>(IFactoryAsync<TOut, TIn> factory, params Type[] extraTypes)
        {
            return FromMethodAsync(Create, extraTypes);

            async UniTask<TOut> Create(IResolutionContext resolver) => await factory.Create(await resolver.Resolve<TIn>());
        }

        public static ResolverFactory FromCloneableT<T>(T cloneable, params Type[] extraTypes)
            where T : ICloneable<T>
        {
            return FromMethodAsync(Create, extraTypes);

            UniTask<T> Create() => new(cloneable.Clone());
        }

        public static ResolverFactory FromCloneable<T>(T cloneable, params Type[] extraTypes)
            where T : ICloneable
        {
            return FromMethodAsync(Create, extraTypes);

            UniTask<object> Create() => new(cloneable.Clone());
        }

        public static ResolverFactory FromCloneableAsync<T>(T cloneable, params Type[] extraTypes)
            where T : ICloneableAsync<T>
        {
            return FromMethodAsync(cloneable.Clone, extraTypes);
        }
    }
}