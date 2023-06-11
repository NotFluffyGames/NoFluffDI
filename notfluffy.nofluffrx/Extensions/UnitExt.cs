using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace NotFluffy.NoFluffRx
{
    public static class UnitExt
    {
        /// <inheritdoc cref="System.ObservableExtensions.Subscribe{T}(IObservable{T}, Action{T})"/>
        public static IDisposable Subscribe(this IObservable<Unit> source, Action onNext) => source.Subscribe(onNext.UnitAction());

        /// <inheritdoc cref="System.ObservableExtensions.Subscribe{T}(IObservable{T}, Action{T}, Action{Exception})"/>
        public static IDisposable Subscribe(this IObservable<Unit> source, Action onNext, Action<Exception> onError) => source.Subscribe(onNext.UnitAction(), onError);

        /// <inheritdoc cref="System.ObservableExtensions.Subscribe{T}(IObservable{T}, Action{T}, Action)"/>
        public static IDisposable Subscribe(this IObservable<Unit> source, Action onNext, Action onCompleted) => source.Subscribe(onNext.UnitAction(), onCompleted);

        /// <inheritdoc cref="System.ObservableExtensions.Subscribe{T}(IObservable{T}, Action{T}, Action{Exception}, Action)"/>
        public static IDisposable Subscribe(this IObservable<Unit> source, Action onNext, Action<Exception> onError, Action onCompleted) => source.Subscribe(onNext.UnitAction(), onError, onCompleted);
        
        public static IDisposable Subscribe(this IObservable<Unit> source, IDisposable disposeOnNext) => source.SubscribeOnce(disposeOnNext.Dispose);
        public static IObservable<Unit> AsUnitObservable<T>(this IObservable<T> source) => source.Select(_ => new Unit());
        public static IDisposable MultiSubscribe(this IObservable<Unit> source, params IDisposable[] disposables)
            => new CompositeDisposable(disposables.Select(source.Subscribe));

        private static Action<Unit> UnitAction(this Action action)
        {
            if (action == null)
                return null;

            return _ => action();
        }
        
        public static void OnNext(this IObserver<Unit> observable)
             => observable.OnNext(new Unit());
        
        /// <inheritdoc cref="System.ObservableExtensions.Subscribe{T}(IObservable{T}, Action{T})"/>
        public static IDisposable SubscribeOnce(this IObservable<Unit> observable, Action onNext)
            => observable.Take(1).Subscribe(onNext);
        /// <inheritdoc cref="System.ObservableExtensions.Subscribe{T}(IObservable{T}, Action{T})"/>
        public static IDisposable SubscribeOnce<T>(this IObservable<T> observable, IObserver<T> observer)
            => observable.Take(1).Subscribe(observer);
        /// <inheritdoc cref="System.ObservableExtensions.Subscribe{T}(IObservable{T}, Action{T})"/>
        public static IDisposable SubscribeOnce<T>(this IObservable<T> observable, Action<T> onNext)
            => observable.Take(1).Subscribe(onNext);
        public static IAwaiter GetAwaiter(this IObservable<Unit> observable)
        {
            var awaiter = new Awaiter();
            observable.LastAsync().Subscribe(awaiter);
            return awaiter;
        }
    }
}
