using System;
using UnityEngine;

namespace NotFluffy.NoFluffDI
{
    [DefaultExecutionOrder(-9999)]
    [DisallowMultipleComponent]
    public class GameObjectContext : MonoContext
    {
        protected override void BindContext(Action<IContainer> callback)
            => this.LazyGetOrCreateScope(callback);
    }
}
