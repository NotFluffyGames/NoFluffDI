using System.Collections.Generic;

namespace NotFluffy
{
    public interface ITreeNode<out T>
        where T : ITreeNode<T>
    {
        T Parent { get; }
        IReadOnlyList<T> Children { get; }
    }
}
