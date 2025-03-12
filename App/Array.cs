using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace App;

// Null dereference checker ignores function calls
// and can't prove that the code is valid.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8601 // Possible null reference assignment.

public class Array<T> : IList<T>
{
    private T[]? data = null;

    public int Count
    {
        get;
        set
        {
            if (value == Count)
                return;

            if (value == 0)
            {
                data = null;
                field = 0;
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

            field = value;
        }
    }

    public int Capacity
    {
        get => data?.Length ?? 0;
        set => Reserve(value);
    }

    public bool IsReadOnly => false;

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

    public Array<T> this[Range range]
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

    public Array()
    { }

    public Array(int capacity)
    {
        data = new T[capacity];
    }

    public Array(int length, T zero) : this(length)
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

    public int IndexOf(T item) => throw new NotImplementedException();

    public void Insert(int index, T item) => throw new NotImplementedException();

    public void RemoveAt(int index) => throw new NotImplementedException();

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
            const int InitialCapacity = 32;

            if (capacity == 0)
                return InitialCapacity;

            if (capacity < GrowThreshold)
                return capacity + capacity;

            return capacity + (capacity / 4);
        }
    }

    public void Clear() => Count = 0;

    public bool Contains(T item) => throw new NotImplementedException();

    public void CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();

    public bool Remove(T item) => throw new NotImplementedException();

    public IEnumerator<T> GetEnumerator() => throw new NotImplementedException();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static Array<T> operator +(Array<T> a, Array<T> b)
    {
        if (a.Count == 0) return b;
        if (b.Count == 0) return a;

        var result = new Array<T> { Count = a.Count + b.Count };

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
