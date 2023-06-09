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

            UniTask<object> Method(IResolutionContext _) => new(instance);
        }

        public static ResolverFactory FromInstance<T>(T instance, params Type[] extraTypes)
        {
            return new ResolverFactory(typeof(T), Method, extraTypes);

            UniTask<object> Method(IResolutionContext _) => new(instance);
        }
    }
}