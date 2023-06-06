using System;
using System.Threading;
using UnityEngine;

namespace NotFluffy.NoFluffDI
{
    [DefaultExecutionOrder(-10000)]
    public abstract class MonoContext : MonoBehaviour
    {
        [SerializeField]
        protected InstallersCollection installers;

        private IReadOnlyContainer _container;

        private readonly CancellationTokenSource _disposeSource = new();

        protected abstract void BindContext(Action<IContainerBuilder> callback);

        protected virtual void Awake()
        {
            BindContext(Callback);

            void Callback(IContainerBuilder builder)
            {
                builder.Install(installers);
                _container = builder.Build();
            }
        }

        protected virtual void OnDestroy()
        {
            //_disposeSource?.Dispose();
        }
    }
}