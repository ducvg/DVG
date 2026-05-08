#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Runtime.CompilerServices;

namespace DVG.StateMachine.Editor
{
    //stack least complexity, tracking
    internal class LiteStack<T>
    {
        private const int MinimumGrow = 4;
        private const int GrowFactor = 2;

        T[] array;
        int tail;
        int size;

        public LiteStack(int capacity)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
            array = new T[capacity];
            tail = size = 0;
        }

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return size; }
        }

        public T Peek()
        {
            if (size == 0) return default;
            return array[tail];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Puah(T item)
        {
            if (size == array.Length)
            {
                Grow();
            }

            array[tail] = item;
            MoveNext();
            size++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Pop()
        {
            if (size == 0) return default;

            T removed = array[tail];
            array[tail] = default;
            MoveNext();
            size--;
            return removed;
        }

        public Span<T> AsSpan()
        {
            return array.AsSpan();
        }

        [MethodImpl(MethodImplOptions.NoInlining)] //rare case
        private void Grow()
        {
            int newcapacity = array.Length * GrowFactor;
            if (newcapacity < array.Length + MinimumGrow)
            {
                newcapacity = array.Length + MinimumGrow;
            }
            
            T[] newarray = new T[newcapacity];
            if (size > 0)
            {
                if (head < tail)
                {
                    Array.Copy(array, head, newarray, 0, size);
                }
                else
                {
                    Array.Copy(array, head, newarray, 0, array.Length - head);
                    Array.Copy(array, 0, newarray, array.Length - head, tail);
                }
            }

            array = newarray;
            tail = (size == newcapacity) ? 0 : size;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void MoveNext()
        {
            int tmp = tail + 1;
            if (tmp == array.Length)
            {
                tmp = 0;
            }
            tail = tmp;
        }
    }
}