using System;
using Cysharp.Threading.Tasks;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace NotFluffy.NoFluffDI
{
    public static class ResolveExt
    {
        public static bool TryResolve<T>(this IReadOnlyContainer container, out T value, object id = null)
        {
            if (container.Contains<T>(id))
            {
                value = container.Resolve<T>(id);
                return true;
            }

            value = default;
            return false;
        }

        public static bool TryResolve(this IReadOnlyContainer container, Type type, out object value, object id = null)
        {
            if (container.Contains(type, id))
            {
                value = container.Resolve(type, id);
                return true;
            }

            value = default;
            return false;
        }

        public static TContract Resolve<TContract>(this IReadOnlyContainer container, object id = null) 
            => (TContract)container.Resolve(typeof(TContract), id);

        public static T Resolve<T>(this IResolutionContext ctx, object id = null) 
            => ctx.Container.Resolve<T>(id);
    }
}