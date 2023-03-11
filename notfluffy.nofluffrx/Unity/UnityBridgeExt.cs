using System;
using System.Reactive;
using UnityEngine;

namespace NotFluffy.NoFluffRx.Unity
{
    public static class UnityBridgeExt
    {
        public static IObservable<Unit> OnDestroy(this GameObject go)
            => OnDestroyBridge.GetOnDestroy(go);
        public static OnDestroyBridge OnDestroy(this Component comp)
            => OnDestroyBridge.GetOnDestroy(comp.gameObject);
    }
}
