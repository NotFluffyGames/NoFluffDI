using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

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
        
        public static IEnumerable<T> Except<T>(this IEnumerable<T> enumerable, T except)
        {
            return enumerable.Where(element => !Equals(element, except));
        }
    }
}