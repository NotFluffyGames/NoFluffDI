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

            UniTask<object> Method(IResolutionContext ctx) => new(method(ctx));
        }

        public static ResolverFactory FromMethod<T>(Func<IResolutionContext, T> method, params Type[] extraTypes)
        {
            return new ResolverFactory(typeof(T), Method, extraTypes);

            UniTask<object> Method(IResolutionContext ctx) => new(method(ctx));
        }

        public static ResolverFactory FromMethodAsync<T>(Func<IResolutionContext, UniTask<T>> method)
        {
            return new ResolverFactory(typeof(T), Method, null);

            async UniTask<object> Method(IResolutionContext ctx) => await method(ctx);
        }

        public static ResolverFactory FromMethodAsync<T>(Func<IResolutionContext, UniTask<T>> method,
            params Type[] extraTypes)
        {
            return new ResolverFactory(typeof(T), Method, extraTypes);

            async UniTask<object> Method(IResolutionContext ctx) => await method(ctx);
        }

        public static ResolverFactory FromMethod<T>(Func<T> method)
        {
            return new ResolverFactory(typeof(T), Method, null);

            UniTask<object> Method(IResolutionContext _) => new(method());
        }

        public static ResolverFactory FromMethod<T>(Func<T> method, params Type[] extraTypes)
        {
            return new ResolverFactory(typeof(T), Method, extraTypes);

            UniTask<object> Method(IResolutionContext _) => new(method());
        }

        public static ResolverFactory FromMethodAsync<T>(Func<UniTask<T>> method)
        {
            return new ResolverFactory(typeof(T), Method, null);

            async UniTask<object> Method(IResolutionContext _) => await method();
        }

        public static ResolverFactory FromMethodAsync<T>(Func<UniTask<T>> method, params Type[] extraTypes)
        {
            return new ResolverFactory(typeof(T), Method, extraTypes);

            async UniTask<object> Method(IResolutionContext _) => await method();
        }

        public static ResolverFactory FromMethod<TParam, T>(Func<IResolutionContext, TParam, T> method)
        {
            return new ResolverFactory(typeof(T), Method, null);

            async UniTask<object> Method(IResolutionContext ctx) => method(ctx, await ctx.Resolve<TParam>());
        }

        public static ResolverFactory FromMethod<TParam, T>(Func<IResolutionContext, TParam, T> method,
            params Type[] extraTypes)
        {
            return new ResolverFactory(typeof(T), Method, extraTypes);

            async UniTask<object> Method(IResolutionContext ctx) => method(ctx, await ctx.Resolve<TParam>());
        }

        public static ResolverFactory FromMethodAsync<TParam, T>(Func<IResolutionContext, TParam, UniTask<T>> method)
        {
            return new ResolverFactory(typeof(T), Method, null);

            async UniTask<object> Method(IResolutionContext ctx) => await method(ctx, await ctx.Resolve<TParam>());
        }

        public static ResolverFactory FromMethodAsync<TParam, T>(Func<IResolutionContext, TParam, UniTask<T>> method,
            params Type[] extraTypes)
        {
            return new ResolverFactory(typeof(T), Method, extraTypes);

            async UniTask<object> Method(IResolutionContext ctx) => await method(ctx, await ctx.Resolve<TParam>());
        }

        public static ResolverFactory FromMethod<TParam, T>(Func<TParam, T> method)
        {
            return new ResolverFactory(typeof(T), Method, null);

            async UniTask<object> Method(IResolutionContext ctx) => method(await ctx.Resolve<TParam>());
        }

        public static ResolverFactory FromMethod<TParam, T>(Func<TParam, T> method, params Type[] extraTypes)
        {
            return new ResolverFactory(typeof(T), Method, extraTypes);

            async UniTask<object> Method(IResolutionContext ctx) => method(await ctx.Resolve<TParam>());
        }

        public static ResolverFactory FromMethodAsync<TParam, T>(Func<TParam, UniTask<T>> method)
        {
            return new ResolverFactory(typeof(T), Method, null);

            async UniTask<object> Method(IResolutionContext ctx) => await method(await ctx.Resolve<TParam>());
        }

        public static ResolverFactory FromMethodAsync<TParam, T>(Func<TParam, UniTask<T>> method,
            params Type[] extraTypes)
        {
            return new ResolverFactory(typeof(T), Method, extraTypes);

            async UniTask<object> Method(IResolutionContext ctx) => await method(await ctx.Resolve<TParam>());
        }
    }
}