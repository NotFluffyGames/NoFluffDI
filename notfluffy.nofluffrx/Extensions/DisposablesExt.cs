using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive;

namespace NotFluffy.NoFluffRx
{
    public static class DisposablesExt
    {
        public static IDisposable Subscribe(this IDisposable disposable, IObservable<Unit> observable)
            => observable.Subscribe(disposable);
    }
}
