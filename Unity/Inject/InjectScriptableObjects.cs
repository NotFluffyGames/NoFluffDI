using System.Linq;
using UnityEngine;

namespace NotFluffy.NoFluffDI.Unity
{
    public abstract class InjectScriptableObjects : MonoInstaller
    {
        [SerializeField] private InjectableScriptableObject[] _injectables;

        public override void InstallBindings(IContainerBuilder builder)
        {
            foreach (var go in _injectables.Distinct())
                builder.AddInjectable(go);
        }
    }
}