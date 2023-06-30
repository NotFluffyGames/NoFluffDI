using System;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace NotFluffy.NoFluffDI.Tests
{
    public static class UniTaskAssertExt
    {
        public static async UniTask AssertThrows<T, TException>(this UniTask<T> task)
        {
            Exception exception = null;
            try
            {
                await task;
            }
            catch (Exception e)
            {
                exception = e;
            }

            Assert.IsInstanceOf<TException>(exception);
        }

        public static async UniTask AssertThrows<TException>(this UniTask task)
            where TException : Exception
        {
            try
            {
                await task;
            }
            catch (TException e)
            {
                Assert.IsInstanceOf<TException>(e);
                return;
            }

            Assert.Fail();
        }
    }
}