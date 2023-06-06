using UnityEngine;

namespace NotFluffy.NoFluffDI
{
    public abstract class MonoInstaller : MonoBehaviour, IInstallable
    {
        public abstract void InstallBindings(IContainerBuilder builder);
    }
}