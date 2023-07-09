using UnityEngine;

namespace NotFluffy.NoFluffDI
{
    [DefaultExecutionOrder(-10000)]
    [DisallowMultipleComponent]
    public class SceneContext : MonoContext
    {
        protected override IContainerBuilder BindContext() 
            => gameObject.scene.CreateScope();
    }
}