using Cysharp.Threading.Tasks;

namespace NotFluffy
{
    public interface IFactory<out TOut, in TIn>
    {
        TOut Create(TIn input);
    }

    public interface IFactoryAsync<TOut, in TIn>
    {
        UniTask<TOut> Create(TIn input);
    }
}
