namespace NotFluffy.NoFluffDI
{
    public static partial class Resolve
    {
        public static ResolverFactory<T> FromInstance<T>(T instance)
        {
            return new ResolverFactory<T>(Method);

            T Method(IResolutionContext _) => instance;
        }
    }
}