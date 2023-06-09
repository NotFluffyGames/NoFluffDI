using System;
using Cysharp.Threading.Tasks;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace NotFluffy.NoFluffDI
{
    public static partial class Resolve
    {
        public static ResolverFactory FromContainer<T>(IReadOnlyContainer container)
        {
            return FromMethodAsync(container.Resolve<T>).AsTransient();
        }

        public static ResolverFactory FromContainer<T>(IReadOnlyContainer container, params Type[] extraTypes)
        {
            return FromMethodAsync(container.Resolve<T>, extraTypes).AsTransient();
        }

        public static ResolverFactory FromContainer<TBind, TResolve>(IReadOnlyContainer container, params Type[] extraTypes)
            where TResolve : TBind
        {
            return FromMethodAsync(ResolveFromContainer, extraTypes).AsTransient();

            async UniTask<TBind> ResolveFromContainer() => await container.Resolve<TResolve>();
        }

        public static ResolverFactory FromContainer<TBind, TResolve>(IReadOnlyContainer container)
            where TResolve : TBind
        {
            return FromMethodAsync(ResolveFromContainer).AsTransient();

            async UniTask<TBind> ResolveFromContainer() => await container.Resolve<TResolve>();
        }

        public static ResolverFactory FromContainer<TBind, TResolve>(IReadOnlyContainer container, Func<TResolve, TBind> selector)
        {
            return FromMethodAsync(ResolveFromContainer).AsTransient();

            async UniTask<TBind> ResolveFromContainer() => selector(await container.Resolve<TResolve>());
        }

        public static ResolverFactory FromContainer<TBind, TResolve>(IReadOnlyContainer container, Func<TResolve, TBind> selector, params Type[] extraTypes)
        {
            return FromMethodAsync(ResolveFromContainer, extraTypes).AsTransient();

            async UniTask<TBind> ResolveFromContainer() => selector(await container.Resolve<TResolve>());
        }

        public static ResolverFactory FromContainerAsync<TBind, TResolve>(IReadOnlyContainer container, Func<TResolve, UniTask<TBind>> selector)
        {
            return FromMethodAsync(ResolveFromContainer).AsTransient();

            async UniTask<UniTask<TBind>> ResolveFromContainer() => selector(await container.Resolve<TResolve>());
        }

        public static ResolverFactory FromContainerAsync<TBind, TResolve>(IReadOnlyContainer container, Func<TResolve, UniTask<TBind>> selector, params Type[] extraTypes)
        {
            return FromMethodAsync(ResolveFromContainer, extraTypes).AsTransient();

            async UniTask<UniTask<TBind>> ResolveFromContainer() => selector(await container.Resolve<TResolve>());
        }
    }
}