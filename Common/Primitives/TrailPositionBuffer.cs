using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvermorrowMod.Common.Primitives
{
    /// <summary>
    /// Simple circular buffer for storing trail positions.
    /// Adapted from https://github.com/joaoportela/CircularBuffer-CSharp
    /// </summary>
    public class TrailPositionBuffer : IReadOnlyList<Vector2>
    {
        private Vector2[] buffer;
        private int start;
        private int size;
        private int end;

        public TrailPositionBuffer(int capacity)
        {
            if (capacity <= 0) throw new ArgumentOutOfRangeException("Capacity must be a positive number");
            buffer = new Vector2[capacity];
            start = 0;
            end = 0;
            size = 0;
        }

        public int Capacity { get => buffer.Length; }

        // We only need push-back and pop-front, so we only add those.
        public void PushBack(Vector2 item)
        {
            if (IsFull)
            {
                buffer[end] = item;
                Increment(ref end);
                start = end;
            }
            else
            {
                buffer[end] = item;
                Increment(ref end);
                ++size;
            }
        }

        public void PopFront()
        {
            if (IsEmpty) throw new InvalidOperationException("Cannot take elements from an empty buffer.");
            Increment(ref start);
            --size;
        }

        public bool IsFull
        {
            get
            {
                return Count == Capacity;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return Count == 0;
            }
        }

        public int Count => size;

        private void Increment(ref int index)
        {
            if (++index == Capacity)
            {
                index = 0;
            }
        }

        private int InternalIndex(int index)
        {
            return start + (index < (Capacity - start) ? index : index - Capacity);
        }

        public IEnumerator<Vector2> GetEnumerator()
        {
            for (int i = start; i < size; i++)
            {
                yield return buffer[i % Capacity];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Vector2 this[int index]
        {
            get
            {
                if (IsEmpty)
                {
                    throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer is empty");
                }
                if (index >= size)
                {
                    throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer size is {size}");
                }
                int actualIndex = InternalIndex(index);
                return buffer[actualIndex];
            }
            set
            {
                if (IsEmpty)
                {
                    throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer is empty");
                }
                if (index >= size)
                {
                    throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer size is {size}");
                }
                int actualIndex = InternalIndex(index);
                buffer[actualIndex] = value;
            }
        }
    }
}
