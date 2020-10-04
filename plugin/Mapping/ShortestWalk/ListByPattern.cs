namespace ShortestWalk.Gh
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal class ListByPattern<T> : IList<T>
    {
        private readonly IList<T> _content;
        private readonly int _innerCount;

        public ListByPattern(IList<T> content, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException("Length is less than 0", "length");

            Count = length;
            _content = content;
            _innerCount = _content.Count;
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException("index");

                return _content[index % _innerCount];
            }
        }

        public int IndexOf(T item)
        {
            var index = _content.IndexOf(item);
            return index < Count ? index : -1;
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException("This list is readonly.");
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException("This list is readonly.");
        }

        T IList<T>.this[int index]
        {
            get => this[index];
            set => throw new NotSupportedException("This list is readonly.");
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException("This list is readonly.");
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException("This list is readonly.");
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex", "The array index must be larger than 0.");

            if (array.Rank != 1)
                throw new ArgumentException("Array rank must be 1, but this array is multi-dimensional.", "array");

            if (arrayIndex >= array.Length ||
                Count + arrayIndex > array.Length)
                throw new ArgumentException("arrayIndex");
            for (var i = 0; i < Count; i++) array[arrayIndex++] = this[i];
        }

        public int Count { get; }

        bool ICollection<T>.IsReadOnly => true;

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException("This list is readonly.");
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; i++) yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}