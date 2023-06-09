using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace NotFluffy.NoFluffDI
{
    public static partial class Resolve
    {
        public static ResolverFactory FromNew<T>() where T : new()
            => FromMethod(() => new T());

        public static ResolverFactory FromNew<TBind, T>() where T : TBind, new()
            => FromMethod<TBind>(() => new T());

        public static ResolverFactory FromNew<T>(params Type[] extraTypes) where T : new()
            => FromMethod(() => new T(), extraTypes);

        public static ResolverFactory FromNew<TBind, T>(params Type[] extraTypes) where T : TBind, new()
            => FromMethod<TBind>(() => new T(), extraTypes);
    }
}