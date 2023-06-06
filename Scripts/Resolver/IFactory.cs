using Cysharp.Threading.Tasks;

namespace NotFluffy
{
    public interface IFactory<T>
    {
        UniTask<T> Create();
    }

    public interface IFactory<TOut, in TIn>
    {
        UniTask<TOut> Create(TIn input);
    }
}
