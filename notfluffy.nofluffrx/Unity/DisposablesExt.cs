using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive;
using UnityEngine;

namespace NotFluffy.NoFluffRx.Unity
{
    public static class DisposablesExt
    {
        public static IDisposable AddTo(this IDisposable disposable, GameObject gameObject)
            => gameObject.OnDestroy().Subscribe(disposable);
        public static IDisposable AddTo(this IDisposable disposable, Component component)
            => component.OnDestroy().Subscribe(disposable);
    }
}
