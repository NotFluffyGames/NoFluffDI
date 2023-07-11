namespace NotFluffy.NoFluffDI
{
    public sealed class ResolverFactory<T> : ResolverFactoryFluent<T, ResolverFactory<T>>
    {
        public ResolverFactory(ResolveMethod<T> method) : base(method)
        {
        }
    }
}