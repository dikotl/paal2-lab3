using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace App;

// Null dereference checker ignores function calls
// and can't prove that the code is valid.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8601 // Possible null reference assignment.

public class DynArray<T> : IList<T>
{
    private T[]? data = null;

    public bool IsReadOnly => false;

    public int Count { get; private set; }

    public int Capacity
    {
        get => data?.Length ?? 0;
        set => Reserve(value);
    }

    public T this[int index]
    {
        get => this[new Index(index)];
        set => this[new Index(index)] = value;
    }

    public T this[Index index]
    {
        get
        {
            if (index.IsFromEnd)
                throw new NotImplementedException("Backward indexing is not implemented");

            if (index.Value >= Count)
                throw new IndexOutOfRangeException();

            return data[index];
        }
        set
        {
            if (index.IsFromEnd)
                throw new NotImplementedException("Backward indexing is not implemented");

            if (index.Value >= Count)
                throw new IndexOutOfRangeException();

            data[index] = value;
        }
    }

    public DynArray<T> this[Range range]
    {
        get
        {
            throw new NotImplementedException("Slicing is not implemented");
        }
        set
        {
            throw new NotImplementedException("Slicing is not implemented");
        }
    }

    public DynArray()
    { }

    public DynArray(int capacity)
    {
        data = new T[capacity];
    }

    public DynArray(int length, T zero) : this(length)
    {
        Count = length;

        for (int i = 0; i < length; i++)
        {
            data[i] = zero;
        }
    }

    public void Reserve(int capacity)
    {
        if (capacity <= Capacity)
            return;

        // Allocate new buffer and copy all elements from old buffer to the new one.
        var newData = new T[capacity];

        for (int i = 0; i < Count; i++)
        {
            newData[i] = data[i];
        }

        data = newData;
    }

    public void Resize(int value)
    {
        if (value == Count)
            return;

        if (value == 0)
        {
            data = null;
            Count = 0;
            return;
        }

        if (value < Count)
        {
            // Just trim the buffer.
            data = data[..value];
        }
        // Buffer may not needed to be reallocated if there is enough capacity.
        else if (value > Capacity)
        {
            Reserve(value);
        }

        for (int i = Count; i < value; i++)
        {
            data[i] = default;
        }

        Count = value;
    }

    public override string ToString()
    {
        // We don't know what length T.ToString() have.
        var buffer = new StringBuilder();
        buffer.Append('[');

        for (int i = 0; i < Count; i++)
        {
            if (i > 0) buffer.Append(", ");
            buffer.Append(data[i].ToString());
        }

        buffer.Append(']');
        return buffer.ToString();
    }

    public int IndexOf(T target)
    {
        for (int i = 0; i < Count; i++)
        {
            if (data[i].Equals(target)) return i;
        }

        return -1;
    }

    public void Insert(int index, T item) => throw new NotImplementedException();

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        // Set the element to default value so we don't hold any data in
        // the baking buffer.
        data[index] = default;
        --Count;

        // Shift all elements after the index by 1.
        for (int i = index; i < Count; i++)
        {
            (data[i + 1], data[i]) = (data[i], data[i + 1]);
        }
    }

    public void Add(T item)
    {
        if (Count == Capacity)
        {
            Reserve(GrowFactor(Capacity));
        }

        data[Count++] = item;


        static int GrowFactor(int capacity)
        {
            const int GrowThreshold = 256;
            const int InitialCapacity = 4;

            if (capacity == 0)
                return InitialCapacity;

            if (capacity < GrowThreshold)
                return capacity + capacity;

            return capacity + (capacity / 4);
        }
    }

    public void Clear()
    {
        Count = 0;
    }

    public bool Contains(T target)
    {
        return IndexOf(target) >= 0;
    }

    public void CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();

    public bool Remove(T target)
    {
        int index = IndexOf(target);

        if (index >= 0)
        {
            RemoveAt(index);
        }

        return index >= 0;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return data[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static DynArray<T> operator +(DynArray<T> a, DynArray<T> b)
    {
        if (a.Count == 0) return b;
        if (b.Count == 0) return a;

        var result = new DynArray<T> { Count = a.Count + b.Count };

        for (int i = 0; i < a.Count; i++)
        {
            result.data[i] = a[i];
        }

        for (int i = a.Count; i < result.Count; i++)
        {
            result.data[i] = b[i];
        }

        result.Count = result.Capacity;
        return result;
    }
}
