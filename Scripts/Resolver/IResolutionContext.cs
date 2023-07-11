namespace NotFluffy.NoFluffDI
{
    public interface IResolutionContext
    {
        IReadOnlyContainer Container { get; }
    }
}