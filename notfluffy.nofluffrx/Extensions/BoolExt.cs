using System;
using System.Reactive;
using System.Reactive.Linq;

namespace NotFluffy.NoFluffRx
{
    public static class BoolExt
    {
        public static IObservable<Unit> WhenTrue(this IObservable<bool> source)
            => source.Where(v => v).AsUnitObservable();

        public static IObservable<Unit> WhenFalse(this IObservable<bool> source)
            => source.Where(v => !v).AsUnitObservable();
    }
}
