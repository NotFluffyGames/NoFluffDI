using System.Collections.Generic;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace NotFluffy.NoFluffDI
{
    public static class BuilderExt
    {
        public static IContainerBuilder Add(this IContainerBuilder builder, params IResolverFactory[] resolverFactories)
        {
            foreach (var resolverFactory in resolverFactories) 
                builder.Add(resolverFactory);
            
            return builder;
        }
        
        public static IContainerBuilder Add(this IContainerBuilder builder, IEnumerable<IResolverFactory> resolverFactories)
        {
            foreach (var resolverFactory in resolverFactories) 
                builder.Add(resolverFactory);

            return builder;
        }

        public static IContainerBuilder AddInjectable(this IContainerBuilder builder, IInjectable injectable)
        {
            return builder.AddInjectable(injectable.Inject);
        }

        public static IContainerBuilder CreateBuilder(
            this IInstallable installable,
            object context = null,
            IReadOnlyContainer parent = null)
        {
            var builder = new ContainerBuilder(context, parent);
            builder.Install(installable);
            return builder;
        }

        public static IContainerBuildResult BuildContainer(
            this IInstallable installable,
            object context = null,
            IReadOnlyContainer parent = null)
        {
            return installable.CreateBuilder(context, parent).Build();
        }
        
        public static IContainerBuildResult BuildContainer(this IResolverFactory resolverFactory, object context = null, IReadOnlyContainer parent = null)
        {
            using var builder = new ContainerBuilder(context, parent);
            builder.Add(resolverFactory);
            return builder.Build();
        }
        
        public static IContainerBuildResult BuildContainer(this IEnumerable<IResolverFactory> resolverFactories, object context= null, IReadOnlyContainer parent = null)
        {
            using var builder = new ContainerBuilder(context, parent);
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
    }
}