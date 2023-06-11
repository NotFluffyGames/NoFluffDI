using System.Linq;
using UnityEngine;

namespace NotFluffy.NoFluffDI.Unity
{
    public sealed class InjectGameObjects : MonoInstaller
    {
        [SerializeField] private GameObject[] _gameObjects;
        
        public override void InstallBindings(IContainerBuilder builder)
        {
            foreach (var go in _gameObjects.Distinct())
            foreach (var injectable in go.GetComponents<IInjectable>())
                builder.AddInjectable(injectable);
        }
    }
}