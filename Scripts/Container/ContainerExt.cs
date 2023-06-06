using System;
using System.Collections.Generic;
// ReSharper disable MemberCanBePrivate.Global

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

        public static bool TryResolve<T>(this IReadOnlyContainer container, out T value, object id = null)
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

        public static TContract Resolve<TContract>(this IReadOnlyContainer container, object id = null) 
            => (TContract)container.Resolve(typeof(TContract), id);

        public static TContract ResolveFromFactory<TContract>(this IReadOnlyContainer container, object id = null)
            => container.Resolve<IFactory<TContract>>(id).Create();

        public static T Resolve<T>(this IResolutionContext ctx, object id = null)
        {
            return ctx.CurrentContainer.Resolve<T>(id);
        }

        public static object Resolve(this IResolutionContext ctx)
            => ctx.ContextResolver.Resolve(ctx);

        public static IContainerBuilder Scope(this IReadOnlyContainer container, string containerName, IInstallable installable)
        {
            var newContainer = container.Scope(containerName);
            newContainer.Install(installable);
            return newContainer;
        }

        public static bool CanConvert<TFrom, TTo>(this IReadOnlyContainer container)
            => container.CanConvert(typeof(TFrom), typeof(TTo));
        
        public static bool CanConvert(this IReadOnlyContainer container, Type from, Type to)
        {
            return container.GetConverter(from, to).Valid;
        }

        public static TTo Convert<TFrom, TTo>(this IReadOnlyContainer container, TFrom from)
            => (TTo)container.Convert(typeof(TFrom), typeof(TTo), from);
        
        public static object Convert(this IReadOnlyContainer container, Type from, Type to, object toConvert)
        {
            var converter = container.GetConverter(from, to);
            
            if(converter.Valid)
                return converter.Converter(toConvert);

            throw new NoMatchingConverterException(from, to);
        }
    }
}