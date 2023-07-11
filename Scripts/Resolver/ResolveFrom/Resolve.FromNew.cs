namespace NotFluffy.NoFluffDI
{
    public static partial class Resolve
    {
        public static ResolverFactory<T> FromNew<T>()
            where T : new()
            => FromMethod(() => new T());

        public static ResolverFactory<T> FromNew<TBind, T>()
            where T : TBind, new()
            => FromMethod(() => new T()).As<TBind>();
    }
}