using System.Collections.Generic;
using UnityEngine;

namespace NotFluffy.NoFluffDI
{
    public abstract class ScriptableInstaller : ScriptableObject, IInstallable
    {
        public abstract IEnumerable<IResolverFactory> GetBindings(IReadOnlyContainer container);
    }
}