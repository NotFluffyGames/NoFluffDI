using System;
using UnityEngine;

namespace NotFluffy.NoFluffDI
{
    public abstract class MonoContext : MonoBehaviour
    {
        [SerializeField]
        protected InstallersCollection installers;

        private IDisposable _containerDisposable;

        protected abstract IContainerBuilder BindContext();

        protected virtual void Awake()
        {
            var builder = BindContext();
            builder.Install(installers);
            var buildResult = builder.Build();
            _containerDisposable = buildResult.ContainerDisposable;
        }

        private void OnDestroy()
        {
            _containerDisposable?.Dispose();
        }
    }
}