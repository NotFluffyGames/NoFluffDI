using System;
using UnityEngine;

namespace NotFluffy.NoFluffDI
{
    [DefaultExecutionOrder(-10000)]
    public abstract class MonoContext : MonoBehaviour
    {
        [SerializeField]
        protected InstallersCollection installers;

        private IContainer _container;

        protected abstract void BindContext(Action<IContainer> callback);

        protected virtual void Awake()
        {
            BindContext(Callback);

            void Callback(IContainer container)
            {
                _container = container;
                _container.Install(installers);
            }
        }

        protected virtual void OnDestroy()
        {
            _container?.Dispose();
        }
    }
}