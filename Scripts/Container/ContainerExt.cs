using System;
using System.Linq;

namespace NotFluffy.NoFluffDI
{
    public static class ContainerExt
    {
        public static void Install(this IContainer container, IInstallable installable)
            => container.Install(installable.GetBindings);

        public static void Install(this IContainer container, Installable installable)
            => container.Install(installable?.Invoke(container));

        public static void InstallSingle(this IContainer container, IResolverFactory resolver)
            => container.Install(Enumerable.Repeat(resolver, 1));

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

        public static T Resolve<T>(this IResolutionContext ctx)
        {
            var resolver = ctx.Resolver;
            var value = resolver.Resolve(ctx);

            if (value is T result)
                return result;

            throw new WrongContractTypeException(resolver.IDs, value.GetType());
        }

        public static object Resolve(this IResolutionContext ctx)
            => ctx.Resolver.Resolve(ctx);

        public static IContainer Scope(this IReadOnlyContainer container, string containerName, IInstallable installable)
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