using System;
using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI
{
    public static partial class Resolve
    {
        public static ResolverFactory<T> FromMethod<T>(ResolveMethod<T> method)
        {
            return new ResolverFactory<T>(method);
        }

        public static AsyncResolverFactory<T> FromMethodAsync<T>(AsyncResolveMethod<T> method)
        {
            return new AsyncResolverFactory<T>(method);
        }

        public static ResolverFactory<T> FromMethod<T>(Func<T> method)
        {
            return new ResolverFactory<T>(Method);

            T Method(IResolutionContext _) => method();
        }

        public static AsyncResolverFactory<T> FromMethodAsync<T>(Func<UniTask<T>> method)
        {
            return new AsyncResolverFactory<T>(Method);

            async UniTask<T> Method(IResolutionContext _) => await method();
        }
    }
}