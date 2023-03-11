using NotFluffy.NoFluffRx;

namespace NotFluffy.HarpoonMadness.Model
{
    public interface IReadOnlyRxProperty<T> : IReadOnlyBehaviourSubject<T>
    {
        T Initial { get; }
    }
    
    public interface IRxProperty<T> : IReadOnlyRxProperty<T>, IBehaviourSubject<T>
    {
        
    }
}