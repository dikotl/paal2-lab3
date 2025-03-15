using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace App;

#pragma warning disable CS8601 // Possible null reference assignment.

public class DynArray<T> : IList<T>, ICloneable
{
    private T[] data = [];

    public bool IsReadOnly => false;

    public int Count { get; private set; }

    public int Capacity
    {
        get => data.Length;
        set => Reserve(value);
    }

    public T this[int index]
    {
        get
        {
            if (index >= Count)
                throw new IndexOutOfRangeException();

            return data[index];
        }
        set
        {
            if (index >= Count)
                throw new IndexOutOfRangeException();

            data[index] = value;
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

    public DynArray<T> Slice(int start, int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        ArgumentOutOfRangeException.ThrowIfLessThan(Count - start, length);

        DynArray<T> slice = new(length) { Count = length };
        Array.Copy(data, start, slice.data, 0, length);
        return slice;
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

    private static int GrowFactor(int capacity)
    {
        const int GrowThreshold = 256;
        const int InitialCapacity = 4;

        if (capacity == 0)
            return InitialCapacity;

        if (capacity < GrowThreshold)
            return capacity + capacity;

        return capacity + (capacity / 4);
    }

    public void Resize(int value)
    {
        if (value == Count)
            return;

        if (value == 0)
        {
            data = [];
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

    public int IndexOf(T target)
    {
        for (int i = 0; i < Count; i++)
        {
            if (data[i]?.Equals(target) ?? false) return i;
        }

        return -1;
    }

    public void Insert(int index, T item)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, Count);
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);

        if (Count == Capacity)
        {
            Reserve(GrowFactor(Capacity));
        }

        ++Count;

        // Shift all elements after the index by 1.
        for (int i = Count; i > index; i--)
        {
            (data[i - 1], data[i]) = (data[i], data[i - 1]);
        }

        data[index] = item;
    }

    public void RemoveAt(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);

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
    }

    public void Clear()
    {
        Count = 0;
    }

    public bool Contains(T target)
    {
        return IndexOf(target) >= 0;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);

        if (array != null && array.Rank != 1)
            throw new RankException($"{typeof(DynArray<T>).Name}.CopyTo expects plain array as an argument");

        Array.Copy(data, 0, array!, arrayIndex, Count);
    }

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

    public override string ToString()
    {
        // We don't know what length T.ToString() have.
        var buffer = new StringBuilder();
        buffer.Append('[');

        for (int i = 0; i < Count; i++)
        {
            if (i > 0) buffer.Append(", ");
            _ = buffer.Append(data[i]?.ToString());
        }

        buffer.Append(']');
        return buffer.ToString();
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is not DynArray<T> other || Count != other.Count)
            return false;

        for (int i = 0; i < Count; i++)
        {
            T? a = this[i];
            T? b = other[i];

            if (!ReferenceEquals(a, b) ||
                !(a != null && a.Equals(b)) ||
                !(b != null && b.Equals(a)))
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        HashCode hash = new();

        foreach (T item in this)
        {
            hash.Add(item);
        }

        return hash.ToHashCode();
    }

    public object Clone()
    {
        return this[..Count];
    }

    public static bool operator ==(DynArray<T>? a, DynArray<T>? b)
    {
        return Equals(a, b);
    }

    public static bool operator !=(DynArray<T>? a, DynArray<T>? b)
    {
        return !Equals(a, b);
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
