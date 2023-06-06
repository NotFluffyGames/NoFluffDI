using System;

namespace NotFluffy.NoFluffDI
{
    public class SceneContext : MonoContext
    {
        protected override void BindContext(Action<IContainerBuilder> callback)
            => callback?.Invoke(gameObject.scene.CreateScope());
    }
}