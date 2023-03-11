using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NotFluffy
{
    public class Awaiter : IAwaiter, IDisposable
    {
        private bool isCompleted;
        private Action continuation;
        
        bool IAwaiter.IsCompleted => isCompleted;

        void IAwaiter.GetResult()
        {
        }

        ~Awaiter()
        {
            Release();
        }

        void INotifyCompletion.OnCompleted(Action continuation)
        {
            if (isCompleted)
                throw new Exception("Awaited a already completed awaiter");
            
            this.continuation = continuation;
        }

        private void Release()
        {
            if (isCompleted)
                return;

            continuation?.Invoke();
            continuation = null;
            isCompleted = true;
        }

        public void Dispose()
        {
            Release();
        }
    }
}
