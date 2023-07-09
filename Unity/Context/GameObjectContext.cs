using UnityEngine;

namespace NotFluffy.NoFluffDI
{
    [DefaultExecutionOrder(-9999)]
    [DisallowMultipleComponent]
    public class GameObjectContext : MonoContext
    {
        protected override IContainerBuilder BindContext() 
            => this.CreateScope();
    }
}
