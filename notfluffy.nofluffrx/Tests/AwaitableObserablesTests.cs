using System;
using NUnit.Framework;
using System.Reactive;
using System.Reactive.Subjects;

namespace NotFluffy.NoFluffRx.Tests
{
    public class AwaitableObservablesTests
    {

        [Test]
        public void Await_UnitObservableIsAwaitedButNotCalled_AwaitShouldNotContinue()
        {
            var unitObservable = new Subject<Unit>();
            var continued = false;
            
            AwaitObservable(unitObservable, () => continued = true);

            var result = continued;
            
            //Release thread
            unitObservable.OnNext();
            
            Assert.False(result);
        }

        [Test]
        public void Await_UnitObservableIsAwaitedAndCalled_AwaitShouldContinue()
        {
            var unitObservable = new Subject<Unit>();
            var continued = false;
            
            AwaitObservable(unitObservable, () => continued = true);
            
            unitObservable.OnNext();
            
            Assert.True(continued);
        }

        private static async void AwaitObservable(IObservable<Unit> unitObservable, Action onContinue)
        {
            await unitObservable;

            onContinue.Invoke();
        }
    }
}
