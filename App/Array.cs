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
        private set;
    }

    public int Capacity
    {
        get => data?.Length ?? 0;
        set => Resize(value);
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

    public void Resize(int newLength, T? zero = default)
    {
        // No need to change anything.
        if (newLength == Count)
            return;

        if (newLength < Count)
        {
            // Just trim the buffer.
            data = data[..newLength];
        }
        // Buffer may not needed to be reallocated if there is enough capacity.
        else if (newLength > Capacity)
        {
            // Allocate new buffer and copy all elements from old buffer to the new one.
            var newData = new T[newLength];

            for (int i = 0; i < Count; i++)
            {
                newData[i] = data[i];
            }

            data = newData;
        }

        for (int i = Count; i < newLength; i++)
        {
            data[i] = zero;
        }

        Count = newLength;
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

    public void Add(T item) => throw new NotImplementedException();

    public void Clear() => throw new NotImplementedException();

    public bool Contains(T item) => throw new NotImplementedException();

    public void CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();

    public bool Remove(T item) => throw new NotImplementedException();

    public IEnumerator<T> GetEnumerator() => throw new NotImplementedException();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
