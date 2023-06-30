using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace NotFluffy.NoFluffDI
{
    public static class AsyncResolveExt
    {
        public static bool TryResolveAsync<T>(this IReadOnlyContainer container, out UniTask<T> value, object id = null)
        {
            if (container.Contains<T>(id))
            {
                value = container.ResolveAsync<T>(id);
                return true;
            }

            value = default;
            return false;
        }

        public static bool TryResolveAsync(this IReadOnlyContainer container, Type type, out object value, object id = null)
        {
            if (container.Contains(type, id))
            {
                value = container.Resolve(type, id);
                return true;
            }

            value = default;
            return false;
        }

        public static async UniTask<TContract> ResolveAsync<TContract>(this IReadOnlyContainer container, object id = null) 
            => (TContract) await container.ResolveAsync(typeof(TContract), id);

        public static UniTask<T> ResolveAsync<T>(this IResolutionContext ctx, object id = null) 
            => ctx.Container.ResolveAsync<T>(id);

    }
}