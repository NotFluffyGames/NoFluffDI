using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using UnityEngine;

namespace NotFluffy.NoFluffRx.Unity
{
    public class OnDestroyBridge : MonoBehaviour, IObservable<Unit>
    {
        private Subject<Unit> onDestroy = new Subject<Unit>();

        public IDisposable Subscribe(IObserver<Unit> observer) 
            => onDestroy?.Subscribe(observer) ?? Disposable.Empty;

        public static OnDestroyBridge GetOnDestroy(GameObject gameObject)
        {
            var onDestroy = gameObject.GetComponent<OnDestroyBridge>();

            if(onDestroy != null)
                return onDestroy;

            onDestroy = gameObject.AddComponent<OnDestroyBridge>();
            return onDestroy;
        }
            
        void OnDestroy()
        {
            onDestroy.OnNext(new Unit());
            onDestroy.OnCompleted();
            onDestroy.Dispose();
            onDestroy = null;
        }
    }
}
