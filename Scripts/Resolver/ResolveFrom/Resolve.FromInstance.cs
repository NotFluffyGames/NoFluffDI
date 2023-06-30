using System;
using Cysharp.Threading.Tasks;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace NotFluffy.NoFluffDI
{
    public static partial class Resolve
    {
        public static ResolverFactory FromInstance<T>(T instance)
        {
            return new ResolverFactory(typeof(T), Method, null);

            object Method(IResolutionContext _) => instance;
        }

        public static ResolverFactory FromInstance<T>(T instance, params Type[] extraTypes)
        {
            return new ResolverFactory(typeof(T), Method, extraTypes);

            object Method(IResolutionContext _) => instance;
        }
    }
}