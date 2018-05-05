using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace ShortestWalk.Gh
{
    class ListByPattern<T> : IList<T>
    {
        readonly IList<T> _content;
        readonly int _size;
        readonly int _innerCount;

        public ListByPattern(IList<T> content, int length)
        {
            if(length < 0)
                throw new ArgumentOutOfRangeException("Length is less than 0", "length");

            _size = length;
            _content = content;
            _innerCount = _content.Count;
        }

        public int IndexOf(T item)
        {
            int index = _content.IndexOf(item);
            return index < _size ? index : -1;
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException("This list is readonly.");
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException("This list is readonly.");
        }

        public T this[int index]
        {
            get
            {
                if(index < 0 || index >= _size)
                    throw new ArgumentOutOfRangeException("index");

                return _content[index % _innerCount];
            }
        }

        T IList<T>.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                throw new NotSupportedException("This list is readonly.");
            }
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
                _size + arrayIndex > array.Length)
            {
                throw new ArgumentException("arrayIndex");
            }
            for (int i = 0; i < _size; i++)
            {
                array[arrayIndex++] = this[i];
            }
        }

        public int Count
        {
            get { return _size; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException("This list is readonly.");
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _size; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
