using System;
using System.Reactive;

namespace NotFluffy.NoFluffRx
{
    public interface IReadOnlyReactiveDisposable
    {
        IObservable<Unit> OnDispose { get; }
    }
    public interface IReactiveDisposable : IDisposable, IReadOnlyReactiveDisposable
    {
    }
}
