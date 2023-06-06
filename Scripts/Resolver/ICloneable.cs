using Cysharp.Threading.Tasks;

namespace NotFluffy
{
    public interface ICloneable<out T> where T : ICloneable<T>
    {
        T Clone();
    }
    
    public interface ICloneableAsync<T> where T : ICloneableAsync<T>
    {
        UniTask<T> Clone();
    }
}