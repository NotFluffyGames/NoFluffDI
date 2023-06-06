using UnityEngine;

namespace NotFluffy.NoFluffDI
{
    public abstract class ScriptableInstaller : ScriptableObject, IInstallable
    {
        public abstract void InstallBindings(IContainerBuilder builder);
    }
}