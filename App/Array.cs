using System;
using System.Text;

namespace App;

// Null dereference checker ignores function calls
// and can't prove that the code is valid.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

public class Array<T>
{
    private T[]? data = null;

    public int Length { get; private set; }

    public int Capacity
    {
        get
        {
            return data.Length;
        }
        set
        {
            if (value == Capacity) return;

            if (value > Capacity)
            {
                var newData = new T[value];

                for (int i = 0; i < Length; i++)
                {
                    newData[i] = data[i];
                }

                Length = value;
                data = newData;
            }
            else
            {
            }
        }
    }

    public T this[Index index]
    {
        get
        {
            if (index.IsFromEnd)
                throw new NotImplementedException("Backward indexing is not implemented");

            if (index.Value >= Length)
                throw new IndexOutOfRangeException();

            return data[index];
        }
        set
        {
            if (index.IsFromEnd)
                throw new NotImplementedException("Backward indexing is not implemented");

            if (index.Value >= Length)
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

    public Array() { }

    public Array(int capacity)
    {
        data = new T[capacity];
    }

    public Array(int length, T zero) : this(length)
    {
        Length = length;

        for (int i = 0; i < length; i++)
        {
            data[i] = zero;
        }
    }

    public void Resize(int newLength)
    {
        // No need to change anything.
        if (newLength == Length)
            return;

        if (newLength < Length)
        {
            // Just trim the buffer.
            data = data[..(Length - 1)];
        }
        // Buffer may not needed to be reallocated if there is enough capacity.
        else if (newLength > Capacity)
        {
            // Allocate new buffer and copy all elements from old buffer to the new one.
            var newData = new T[newLength];

            for (int i = 0; i < Length; i++)
            {
                newData[i] = data[i];
            }

            data = newData;
        }

        Length = newLength;
    }

    public override string ToString()
    {
        // We don't know what length T.ToString() have.
        var buffer = new StringBuilder();
        buffer.Append('[');

        for (int i = 0; i < Length; i++)
        {
            buffer.Append(data[i].ToString());
        }

        buffer.Append(']');
        return buffer.ToString();
    }
}
