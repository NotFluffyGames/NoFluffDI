using System.Collections.Generic;

namespace NotFluffy.NoFluffDI
{
    public delegate IEnumerable<IResolverFactory> Installable(IReadOnlyContainer container);

    public interface IInstallable
    {
        IEnumerable<IResolverFactory> GetBindings(IReadOnlyContainer container);
    }
}