using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace NotFluffy.NoFluffRx
{
    public class ObservablesGroup<T> : IObservable<T>
    {
        private Func<IReadOnlyCollection<T>, T> selector;
        private Subject<T> subject = new();

        private int observablesCount = 0;
        private Dictionary<IObservable<T>, T> lastValues = new Dictionary<IObservable<T>, T>();

        public ObservablesGroup(Func<IReadOnlyCollection<T>, T> selector)
        {
            this.selector = selector ?? throw new ArgumentNullException(nameof(selector));
            UpdateSubject();
        }

        private bool AllObservablesReturnedValues => observablesCount == lastValues.Count;
        private T CombinedValue => selector(lastValues.Values);
        public IDisposable Subscribe(IObserver<T> observer)
            => subject.Subscribe(observer);

        public IDisposable AddValue(T value) => AddSource(new BehaviorSubject<T>(value));
        public IDisposable AddSource(IObservable<T> source)
        {
            observablesCount++;
            var sub = source.Subscribe(OnNext);

            return Disposable.Create(Dispose);

            void OnNext(T value)
            {
                if (lastValues.ContainsKey(source))
                    lastValues[source] = value;
                else
                    lastValues.Add(source, value);

                UpdateSubject();
            }
            void Dispose()
            {
                sub?.Dispose();
                observablesCount--;
                UpdateSubject();
            }
        }

        private void UpdateSubject()
        {
            if(AllObservablesReturnedValues)
                subject.OnNext(CombinedValue);
        }
    }
}
