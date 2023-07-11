namespace NotFluffy.NoFluffDI
{
    public class AsyncResolverFactory<T> : AsyncResolverFactoryFluent<T, AsyncResolverFactory<T>>
    {
        public AsyncResolverFactory(AsyncResolveMethod<T> method) 
            : base(method)
        {
        }
    }
}