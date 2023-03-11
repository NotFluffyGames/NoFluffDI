namespace NotFluffy
{
    public interface IFactory<out T>
    {
        T Create();
    }

    public interface IFactory<out TOut, in TIn>
    {
        TOut Create(TIn input);
    }
}
