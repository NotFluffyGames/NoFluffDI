using UnityEngine;

namespace NotFluffy.NoFluffDI.Unity
{
    public abstract class InjectComponents : MonoInstaller
    {
        [SerializeField] private InjectableComponent[] _injectables;

        public override void InstallBindings(IContainerBuilder builder)
        {
            foreach (var injectable in _injectables)
                builder.AddInjectable(injectable);
        }
    }
}