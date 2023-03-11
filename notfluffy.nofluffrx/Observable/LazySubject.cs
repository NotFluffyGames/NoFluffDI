using System;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace NotFluffy.NoFluffRx
{
    public class LazySubject<T> : ISubject<T>, IDisposable
    {
        private Subject<T> subject;

        private Action onHot;
        private Action onCold;

        private bool disposed;
        private bool isHot;

        public LazySubject(Action onHot, Action onCold)
        {
            this.onHot = onHot;
            this.onCold = onCold;
        }

        public void Dispose()
        {
            disposed = true;

            subject.Dispose();
            subject = null;

            if(isHot)
                onCold?.Invoke();

            isHot = false;
            onHot = null;
            onCold = null;
        }

        public void OnCompleted() => subject?.OnCompleted();

        public void OnError(Exception error) => subject?.OnError(error);

        public void OnNext(T value) => subject?.OnNext(value);

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (disposed)
                return Disposable.Empty;

            if (subject == null)
                subject = new Subject<T>();

            var sub = subject.Subscribe(observer);

            if (!isHot)
                onHot?.Invoke();

            return Disposable.Create(Unsubscribe);

            void Unsubscribe()
            {
                sub?.Dispose();
                sub = null;

                if(isHot && !subject.HasObservers)
                {
                    isHot = false;
                    onCold?.Invoke();
                }
            }
        }
    }
}
