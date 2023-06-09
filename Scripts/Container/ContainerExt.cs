using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace NotFluffy.NoFluffDI
{
    public static class ContainerExt
    {
        public static void Add(this IContainerBuilder builder, params IResolverFactory[] resolverFactories)
        {
            foreach (var resolverFactory in resolverFactories) 
                builder.Add(resolverFactory);
        }
        
        public static void Add(this IContainerBuilder builder, IEnumerable<IResolverFactory> resolverFactories)
        {
            foreach (var resolverFactory in resolverFactories) 
                builder.Add(resolverFactory);
        }

        public static IReadOnlyContainer CreateContainer(this IInstallable installable, object context= null, IReadOnlyContainer parent = null)
        {
            var builder = new ContainerBuilder(context, parent);
            builder.Install(installable);
            return builder.Build();
        }
        
        public static IReadOnlyContainer CreateContainer(this IResolverFactory resolverFactory, object context = null, IReadOnlyContainer parent = null)
        {
            var builder = new ContainerBuilder(context, parent);
            builder.Add(resolverFactory);
            return builder.Build();
        }
        
        public static IReadOnlyContainer CreateContainer(this IEnumerable<IResolverFactory> resolverFactories, object context= null, IReadOnlyContainer parent = null)
        {
            var builder = new ContainerBuilder(context, parent);
            builder.Add(resolverFactories);
            return builder.Build();
        }
        
        public static IContainerBuilder Install(this IContainerBuilder builder, IInstallable installable)
        {
            installable.InstallBindings(builder);
            return builder;
        }

        public static IContainerBuilder Install(this IContainerBuilder builder, Installable installable)
        {
            installable?.Invoke(builder);
            return builder;
        }

        public static bool TryResolve<T>(this IReadOnlyContainer container, out UniTask<T> value, object id = null)
        {
            if (Contains<T>(container, id))
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

        public static bool Contains<T>(this IReadOnlyContainer container, object id = null)
            => container.Contains(typeof(T), id);

        public static async UniTask<TContract> Resolve<TContract>(this IReadOnlyContainer container, object id = null) 
            => (TContract) await container.Resolve(typeof(TContract), id);

        public static UniTask<T> Resolve<T>(this IResolutionContext ctx, object id = null) 
            => ctx.Container.Resolve<T>(id);

        public static UniTask<object> Resolve(this IResolutionContext ctx)
            => ctx.ContextResolver.Resolve(ctx);

        public static IContainerBuilder Scope(this IReadOnlyContainer container, string containerName, IInstallable installable)
        {
            var newContainer = container.Scope(containerName);
            newContainer.Install(installable);
            return newContainer;
        }
    }
}