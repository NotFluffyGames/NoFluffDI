using System;
using UnityEngine;

namespace NotFluffy.NoFluffDI
{
    [DefaultExecutionOrder(-10000)]
    [DisallowMultipleComponent]
    public class SceneContext : MonoContext
    {
        protected override void BindContext(Action<IContainerBuilder> callback)
            => callback?.Invoke(gameObject.scene.CreateScope());
    }
}