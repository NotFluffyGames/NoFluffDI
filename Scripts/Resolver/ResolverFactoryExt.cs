using System;

namespace NotFluffy.NoFluffDI
{
    public static class ResolverFactoryExt
    {
        public static TFactory BindDisposable<T, TFactory>(this TFactory factory)
            where TFactory : ResolverFactoryFluent<T, TFactory>
            where T : IDisposable
        {
            return factory.AddPostDisposeAction(obj => (obj as IDisposable)?.Dispose());
        }
    }
}