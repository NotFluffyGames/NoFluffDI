using System.Runtime.CompilerServices;

namespace NotFluffy
{
    public interface IAwaitable
    {
        IAwaiter GetAwaiter();
    }
    public interface IAwaiter : INotifyCompletion
    {
        bool IsCompleted { get; }

        void GetResult();
    }

    public interface IAwaitable<T>
    {
        IAwaiter<T> GetAwaiter();
    }
    public interface IAwaiter<T> : INotifyCompletion
    {
        bool IsCompleted { get; }

        T GetResult();
    }
}
