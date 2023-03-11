using System.Collections.Generic;
using UnityEngine;

namespace NotFluffy.NoFluffDI
{
    public abstract class MonoInstaller : MonoBehaviour, IInstallable
    {
        public abstract IEnumerable<IResolverFactory> GetBindings(IReadOnlyContainer container);
    }
}