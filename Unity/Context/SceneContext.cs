using System;

namespace NotFluffy.NoFluffDI
{
    public class SceneContext : MonoContext
    {
        protected override void BindContext(Action<IContainer> callback) 
            => callback?.Invoke(gameObject.scene.GetOrCreateScope());
    }
}