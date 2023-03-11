using System;
using System.Collections;
using System.Collections.Generic;

namespace NotFluffy.NoFluffRx
{
    public class ImmutableList<T> : IReadOnlyList<T>
    {
        public static readonly ImmutableList<T> Empty = new();

        private readonly T[] _data;
        
        public ImmutableList(T[] data = null) => _data = data ?? Array.Empty<T>();

        public int Count => _data.Length;

        public T this[int index] => _data[index];

        public ImmutableList<T> Add(T value)
        {
            var newData = new T[_data.Length + 1];

            Array.Copy(_data, newData, _data.Length);
            newData[_data.Length] = value;

            return new ImmutableList<T>(newData);
        }

        public ImmutableList<T> Remove(T value)
        {
            var i = Array.IndexOf(_data, value);
            if (i < 0)
            {
                return this;
            }

            var length = _data.Length;
            if (length == 1)
            {
                return Empty;
            }

            var newData = new T[length - 1];

            Array.Copy(_data, 0, newData, 0, i);
            Array.Copy(_data, i + 1, newData, i, length - i - 1);

            return new ImmutableList<T>(newData);
        }

        public IEnumerator<T> GetEnumerator() => ((IReadOnlyList<T>)_data).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();
    }
}
