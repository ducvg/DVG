#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Runtime.CompilerServices;

namespace DVG.StateMachine.Editor
{
    //stack has least complexity, footprint
    internal class LiteStack<T>
    {
        private T[] _array;
        private int _size;

        public LiteStack(int capacity = 0)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
            _array = new T[capacity];
            _size = 0;
        }

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _size; }
        }

        public T Peek()
        {
            if (_size == 0) return default;
            return _array[_size - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(T item)
        {
            if ((uint)_size < (uint)_array.Length)
            {
                _array[_size] = item;
                ++_size;
            }
            else
            {
                const int arrayMaxLength = 0X7FFFFFC7;
                ++_size;

                int newcapacity = _array.Length == 0 ? 16 : _array.Length * 2;
                if ((uint)newcapacity > arrayMaxLength) newcapacity = arrayMaxLength;
                if (newcapacity < _size) newcapacity = _size;
                
                Array.Resize(ref _array, newcapacity);
                
                _array[_size-1] = item;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Pop()
        {
            if (_size == 0) return default;
            --_size;

            T removed = _array[_size];
            _array[_size] = default;
            return removed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan()
        {
            return _array.AsSpan(0, _size);
        }
    }
}