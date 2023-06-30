// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace NotFluffy.NoFluffDI
{
    public static class ContainerExt
    {
        public static bool Contains<T>(this IReadOnlyContainer container, object id = null)
            => container.Contains(typeof(T), id);
    }
}