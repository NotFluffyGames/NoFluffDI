using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace NotFluffy.NoFluffDI
{
    public static class ResolverFactoryExt
    {
        public static TFactory ChainDisposable<TFactory>(this TFactory factory, IDisposable disposable)
            where TFactory : IResolverFactory
        {
            factory.AddOnDisposeAction(_ => disposable?.Dispose());
            return factory;
        }
        
        public static TFactory BindDisposable<T, TFactory>(this TFactory factory)
            where TFactory : IResolverFactory<T>
            where T : IDisposable
        {
            factory.AddOnDisposeAction(obj => obj?.Dispose());
            return factory;
        }

        /// <summary>
        /// Will resolve for T and inject him, T must be resolvable, also will do this once, not for every instance
        /// </summary>
        /// <param name="builder"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IContainerBuilder BindInjectableFromResolve<T>(this IContainerBuilder builder)
            where T : IInjectable
        {
            return builder.AddInjectable(context => context.Container.Resolve<T>().Inject(context));
        }

        public static CancellationToken DisposeAsToken(this IResolverFactory factory)
        {
            var source = new CancellationTokenSource();
            factory.ChainDisposable(source.CancelAsDisposable(true));
            return source.Token;
        }
        
        public static TFactory BindAsStartable<T, TFactory>(this TFactory factory)
            where TFactory : IResolverFactory<T>
            where T : IStartable
        {
            factory.AddPostResolveAction((resolved, context) => resolved.StartAsync(context, factory.DisposeAsToken()));
            return factory;
        }

        public static TFactory BindAsInitializable<T, TFactory>(this TFactory factory)
            where TFactory : IAsyncResolverFactory<T>
            where T : IInitializable
        {
            factory.AddAsyncPostResolveAction((resolved, context) => resolved.Initialize(context, factory.DisposeAsToken()));
            return factory;
        }
    }
}