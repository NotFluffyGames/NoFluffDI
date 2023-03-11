using System.Collections.Generic;

namespace NotFluffy
{
    public static class IEnumerableExt
    {
        public static IEnumerable<T> Reversed<T>(this IList<T> items)
        {
            for (var i = items.Count - 1; i >= 0; i--)
            {
                yield return items[i];
            }
        }

        public delegate bool TrySelect<in TIn, TOut>(TIn input, out TOut output);

        public static IEnumerable<TOut> SelectWhile<TIn, TOut>(this IEnumerable<TIn> collection,
            TrySelect<TIn, TOut> selector)
        {
            foreach (var item in collection)
                if (selector(item, out var result))
                    yield return result;
        }
    }
}