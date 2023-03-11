using NotFluffy.NoFluffRx;

namespace NotFluffy.HarpoonMadness.Model
{
    public sealed class Model<T> : BehaviourSubject<T>, IRxProperty<T>
    {
        public Model(T initial) : base(initial)
        {
            Initial = initial;
        }

        public T Initial { get; }
    }
}