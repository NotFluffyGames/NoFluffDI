using System.Collections.Generic;
using System.Linq;

namespace NotFluffy
{
    public static class TreeExt
    {
        public static bool IsLeaf<T>(this ITreeNode<T> node) where T : ITreeNode<T>
        {
            return node != null && node.Children.Count == 0;
        }

        public static bool HasChildren<T>(this ITreeNode<T> node) where T : ITreeNode<T>
        {
            return node is { Children: { Count: > 0 } };
        }

        public static bool IsChildOf<T>(this ITreeNode<T> node, T parent) where T : ITreeNode<T>
        {
            return node.Parents().Contains(parent);
        }

        public static int Depth<T>(this ITreeNode<T> node) where T : ITreeNode<T>
        {
            return node.Parents().Count();
        }

        public static IEnumerable<T> SelfAndParents<T>(this ITreeNode<T> node)
            where T : ITreeNode<T>
        {
            if (node == null)
                yield break;

            var current = (T)node;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }

        public static IEnumerable<T> Parents<T>(this ITreeNode<T> node) where T : ITreeNode<T>
        {
            if(node == null)
                yield break;
            
            var current = node.Parent;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }

        public static IEnumerable<T> DeepChildrenDfs<T>(this ITreeNode<T> node) where T : ITreeNode<T>
        {
            foreach (var child in node.Children)
            {
                foreach (var deepChild in child.DeepChildrenDfs())
                        yield return deepChild;

                yield return child;
            }
        }

        public static IEnumerable<T> DeepChildrenBfs<T>(this ITreeNode<T> node) where T : ITreeNode<T>
        {
            var queue = new Queue<T>();

            foreach (var child in node.Children) 
                queue.Enqueue(child);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current;

                if(current.Children != null)
                    foreach (var deepChild in current.Children)
                        queue.Enqueue(deepChild);
            }
        }

        public static List<T> ToListTree<T>(this T root) where T : ITreeNode<T>
        {
            return root.DeepChildrenBfs().Prepend(root).ToList();
        }

        public static bool RecursiveGet<T, TResult>(this T node, TryNodeAction<T, TResult> nodeAction, out TResult result) where T : ITreeNode<T>
        {
            while (true)
            {
                if (nodeAction(node, out result)) 
                    return true;

                if (node.Parent != null)
                    node = node.Parent;
                else
                    return false;
            }
        }
    }
    public delegate bool TryNodeAction<in T, TResult>(T node, out TResult result)
        where T : ITreeNode<T>;
}
