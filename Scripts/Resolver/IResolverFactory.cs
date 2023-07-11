namespace NotFluffy.NoFluffDI
{
    public interface IResolverFactory
    {
        IAsyncResolver Create();

        void AsSingle();
        void AsTransient();
        void WithID(object id);
        void As<TType>();
	    
        void AddOnDisposeAction(OnDisposeAction action);
        void AddPostResolveAction(PostResolveAction action);
    }
    
    public interface IResolverFactory<out T> : IResolverFactory
    {
        void AddOnDisposeAction(OnDisposeAction<T> action);
        void AddPostResolveAction(PostResolveAction<T> action);
    }

    public interface IAsyncResolverFactory : IResolverFactory
    {
        void AddAsyncPostResolveAction(AsyncPostResolveAction action);
    }

    public interface IAsyncResolverFactory<out T> : IAsyncResolverFactory, IResolverFactory<T>
    {
        void AddAsyncPostResolveAction(AsyncPostResolveAction<T> action);
    }
}