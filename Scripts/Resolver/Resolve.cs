using System;
using Cysharp.Threading.Tasks;
// ReSharper disable UnusedMember.Global

namespace NotFluffy.NoFluffDI
{
    public static class Resolve
    {
        public static ResolverFactory FromNew<T>(params Type[] extraTypes) where T : new()
            => FromMethod(() => new UniTask<T>(new T()), extraTypes);
        
        public static ResolverFactory FromNew<TBind, T>(params Type[] extraTypes) where T : TBind, new()
            => FromMethod(() => new UniTask<TBind>(new T()), extraTypes);

        public static ResolverFactory FromMethod<T>(Func<IResolutionContext, UniTask<T>> method, params Type[] extraTypes)
        {
            return new ResolverFactory(typeof(T), Method, extraTypes);

            async UniTask<object> Method(IResolutionContext ctx) => await method(ctx);
        }
        
        public static ResolverFactory FromMethod<T>(Func<UniTask<T>> method, params Type[] extraTypes)
        {
            return new ResolverFactory(typeof(T), Method, extraTypes);

            async UniTask<object> Method(IResolutionContext _) => await method();
        }

        public static ResolverFactory FromInstance<T>(T instance, params Type[] extraTypes)
        {
            return new ResolverFactory(typeof(T), Method, extraTypes);

            UniTask<object> Method(IResolutionContext _) => new(instance);
        }

        public static ResolverFactory FromContainer<T>(IReadOnlyContainer container, params Type[] extraTypes)
        {
            return FromMethod(() => container.Resolve<T>(), extraTypes).AsTransient();
        }
        
        public static ResolverFactory FromContainer<TBind, TResolve>(IReadOnlyContainer container, Func<TResolve, UniTask<TBind>> selector, params Type[] extraTypes)
        {
            return FromMethod(async () => selector( await container.Resolve<TResolve>()), extraTypes).AsTransient();
        }

        public static ResolverFactory FromFactory<T>(IFactory<T> factory, params Type[] extraTypes)
        {
            return FromMethod(factory.Create, extraTypes);
        }

        public static ResolverFactory FromCloneableT<T>(T cloneable, params Type[] extraTypes)
            where T : ICloneable<T>
        {
            return FromMethod(() => new UniTask<T>(cloneable.Clone()), extraTypes);
        }

        public static ResolverFactory FromCloneable<T>(T cloneable, params Type[] extraTypes)
            where T : ICloneable
        {
            return FromMethod(() => new UniTask<object>(cloneable.Clone()), extraTypes);
        }
        
        public static ResolverFactory FromCloneableAsync<T>(T cloneable, params Type[] extraTypes)
            where T : ICloneableAsync<T>
        {
            return FromMethod(cloneable.Clone, extraTypes);
        }
    }
}