using System;
using Cysharp.Threading.Tasks;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace NotFluffy.NoFluffDI
{
    public static partial class Resolve
    {
        public static ResolverFactory FromMethod<T>(Func<IResolutionContext, T> method)
        {
            return new ResolverFactory(typeof(T), Method, null);

            object Method(IResolutionContext ctx) => method(ctx);
        }

        public static ResolverFactory FromMethod<T>(Func<IResolutionContext, T> method, params Type[] extraTypes)
        {
            return new ResolverFactory(typeof(T), Method, extraTypes);

            object Method(IResolutionContext ctx) => method(ctx);
        }

        public static AsyncResolverFactory FromMethodAsync<T>(Func<IResolutionContext, UniTask<T>> method)
        {
            return new AsyncResolverFactory(typeof(T), Method, null);

            async UniTask<object> Method(IResolutionContext ctx) => await method(ctx);
        }

        public static AsyncResolverFactory FromMethodAsync<T>(Func<IResolutionContext, UniTask<T>> method, params Type[] extraTypes)
        {
            return new AsyncResolverFactory(typeof(T), Method, extraTypes);

            async UniTask<object> Method(IResolutionContext ctx) => await method(ctx);
        }

        public static ResolverFactory FromMethod<T>(Func<T> method)
        {
            return new ResolverFactory(typeof(T), Method, null);

            object Method(IResolutionContext _) => method();
        }

        public static ResolverFactory FromMethod<T>(Func<T> method, params Type[] extraTypes)
        {
            return new ResolverFactory(typeof(T), Method, extraTypes);

            object Method(IResolutionContext _) => method();
        }

        public static AsyncResolverFactory FromMethodAsync<T>(Func<UniTask<T>> method)
        {
            return new AsyncResolverFactory(typeof(T), Method, null);

            async UniTask<object> Method(IResolutionContext _) => await method();
        }

        public static AsyncResolverFactory FromMethodAsync<T>(Func<UniTask<T>> method, params Type[] extraTypes)
        {
            return new AsyncResolverFactory(typeof(T), Method, extraTypes);

            async UniTask<object> Method(IResolutionContext _) => await method();
        }
    }
}