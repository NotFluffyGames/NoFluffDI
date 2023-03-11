using System;

namespace NotFluffy.NoFluffDI
{
    public static class Resolve
    {
        public static ResolverFactory FromNew<T>(params Type[] extraTypes) where T : new()
            => FromMethod(() => new T(), extraTypes);
        
        public static ResolverFactory FromNew<TBind, T>(params Type[] extraTypes) where T : TBind, new()
            => FromMethod<TBind>(() => new T(), extraTypes);

        public static ResolverFactory FromMethod<T>(Func<IResolutionContext, T> method, params Type[] extraTypes)
        {
            return new ResolverFactory(typeof(T), Method, extraTypes);

            object Method(IResolutionContext ctx) => method(ctx);
        }
        
        public static ResolverFactory FromMethod<T>(Func<T> method, params Type[] extraTypes)
        {
            return new ResolverFactory(typeof(T), Method, extraTypes);

            object Method(IResolutionContext _) => method();
        }

        public static ResolverFactory FromInstance<T>(T instance, params Type[] extraTypes)
        {
            return new ResolverFactory(typeof(T), Method, extraTypes);

            object Method(IResolutionContext _) => instance;
        }

        public static ResolverFactory FromContainer<T>(IReadOnlyContainer container, params Type[] extraTypes)
        {
            return FromMethod(() => container.Resolve<T>(), extraTypes).AsTransient();
        }
        
        public static ResolverFactory FromContainer<TBind, TResolve>(IReadOnlyContainer container, Func<TResolve, TBind> selector, params Type[] extraTypes)
        {
            return FromMethod(() => selector(container.Resolve<TResolve>()), extraTypes).AsTransient();
        }

        public static ResolverFactory FromFactory<T>(IFactory<T> factory, params Type[] extraTypes)
        {
            return FromMethod(factory.Create, extraTypes);
        }

        public static ResolverFactory FromCloneableT<T>(T cloneable, params Type[] extraTypes)
            where T : ICloneable<T>
        {
            return FromMethod(cloneable.Clone, extraTypes);
        }

        public static ResolverFactory FromCloneable<T>(T cloneable, params Type[] extraTypes)
            where T : ICloneable
        {
            return FromMethod(cloneable.Clone, extraTypes);
        }
    }
}