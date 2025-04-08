using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClassLibraryCS.FunctionalEnumerableOperations;

namespace ClassLibraryCS.Collections;

/// <summary>
/// A dynamic array that automatically resizes itself as elements are added or removed.
/// </summary>
public class DynArray<T> : IList<T>, ICloneable
{
    private T?[] data = [];

    /// <summary>
    /// Gets a value indicating whether the collection is read-only.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Checks if the type T is a array by determining if it is assignable from <see cref="ICollection"/>. Returns true if T is a
    /// jagged array type.
    /// </summary>
    private readonly bool IsJagged;

    /// <summary>
    /// Gets the number of elements currently in the collection.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Gets or sets the capacity of the array. The capacity is the number of elements the array can hold before resizing is required.
    /// </summary>
    public int Capacity
    {
        get => data.Length;
        set => Reserve(value);
    }

    /// <summary>
    /// Indexer for accessing elements in the collection.
    /// </summary>
    /// <param name="index">The index of the element in the collection.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown if the index is out of bounds of the current count of elements in the collection.</exception>
    public T this[int index]
    {
        get
        {
            if (index >= Count || index < 0)
                throw new IndexOutOfRangeException($"Index {index} was outside the bounds of the array (length = {Count}).");

            return data[index]!;
        }
        set
        {
            if (index >= Count || index < 0)
                throw new IndexOutOfRangeException($"Index {index} was outside the bounds of the array (length = {Count}).");

            data[index] = value;
        }
    }

    /// <summary>
    /// Initializes a new empty dynamic array.
    /// </summary>
    public DynArray()
    { }

    /// <summary>
    /// Initializes a dynamic array with a specified capacity.
    /// </summary>
    /// <param name="capacity">The capacity of the array.</param>
    public DynArray(int capacity)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Negative capacity is not a valid");

        IsJagged = typeof(T).GetInterfaces().Any(i =>
            i.IsGenericType &&
            i.GetGenericTypeDefinition() == typeof(ICollection<>));

        data = new T[capacity];
    }

    /// <summary>
    /// Initializes a dynamic array with the specified length and fills it with <paramref name="zero"/>.
    /// </summary>
    /// <param name="length">The length of the array.</param>
    /// <param name="zero">The default value for each element in the array. Defaults to null for reference types.</param>
    public DynArray(int length, T? zero = default) : this(length)
    {
        Count = length;

        for (int i = 0; i < length; i++)
        {
            data[i] = zero;
        }
    }

    /// <summary>
    /// Initializes a dynamic array with the specified length and fills it using provided <paramref name="getItem"/> function.
    /// </summary>
    /// <param name="length">The length of the array.</param>
    /// <param name="getItem">A function that generates the value for each element.</param>
    public DynArray(int length, Func<T> getItem) : this(length)
    {
        Count = length;

        for (int i = 0; i < length; i++)
        {
            data[i] = getItem();
        }
    }

    /// <summary>
    /// Creates a slice (sub-array) from the current array starting at a specific index with a given length.
    /// </summary>
    /// <param name="start">The starting index of the slice.</param>
    /// <param name="length">The length of the slice.</param>
    /// <returns>A new `DynArray` containing the sliced portion of the array.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the start or length is negative, or if the slice exceeds the current array bounds.</exception>
    public DynArray<T> Slice(int start, int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        ArgumentOutOfRangeException.ThrowIfLessThan(Count - start, length);

        DynArray<T> slice = new(length) { Count = length };
        Array.Copy(data, start, slice.data, 0, length);
        return slice;
    }

    /// <summary>
    /// Reserves space for a minimum specified capacity in the array.
    /// </summary>
    /// <param name="capacity">The minimum capacity to reserve.</param>
    public void Reserve(int capacity)
    {
        if (capacity <= Capacity)
            return;

        // Allocate new buffer and copy all elements from old buffer to the new one.
        var newData = new T[capacity];

        for (int i = 0; i < Count; i++)
        {
            newData[i] = data[i]!;
        }

        data = newData;
    }

    /// <summary>
    /// Calculates the growth factor for resizing the array.
    /// </summary>
    /// <param name="capacity">The current capacity of the array.</param>
    /// <returns>The new capacity based on the current capacity.</returns>
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

    /// <summary>
    /// Resizes the array to a specified size. If the size is smaller than the current size, elements will be removed.
    /// </summary>
    /// <param name="value">The new size of the array.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is negative.</exception>
    public void Resize(int value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be >= 0");

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

    /// <summary>
    /// Finds the first occurrence of the specified element in the array.
    /// </summary>
    /// <param name="target">The element to find.</param>
    /// <returns>The index of the element, or -1 if not found.</returns>
    public int IndexOf(T target)
    {
        for (int i = 0; i < Count; i++)
        {
            if (data[i]?.Equals(target) ?? false) return i;
        }

        return -1;
    }

    /// <summary>
    /// Finds the last occurrence of the specified element in the array.
    /// </summary>
    /// <param name="target">The element to find.</param>
    /// <returns>The index of the last occurrence of the element, or -1 if not found.</returns>
    public int LastIndexOf(T target)
    {
        for (int i = Count - 1; i >= 0; i--)
        {
            if (data[i]?.Equals(target) ?? false) return i;
        }

        return -1;
    }

    /// <summary>
    /// Inserts an element at the specified index in the array.
    /// </summary>
    /// <param name="index">The index at which to insert the element.</param>
    /// <param name="item">The element to insert.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is outside the bounds of the array.</exception>
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
        for (int i = int.Min(Count, Capacity - 1); i > index; i--)
        {
            (data[i - 1], data[i]) = (data[i], data[i - 1]);
        }

        data[index] = item;
    }

    /// <summary>
    /// Removes the element at the specified index in the array.
    /// </summary>
    /// <param name="index">The index of the element to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is outside the bounds of the array.</exception>
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

    /// <summary>
    /// Adds a new element to the end of the array.
    /// </summary>
    /// <param name="item">The element to add.</param>
    public void Add(T item)
    {
        if (Count == Capacity)
        {
            Reserve(GrowFactor(Capacity));
        }

        data[Count++] = item;
    }

    /// <summary>
    /// Clears all elements in the array, resetting the count to zero.
    /// </summary>
    public void Clear()
    {
        Count = 0;
    }

    /// <summary>
    /// Determines whether the array contains the specified element.
    /// </summary>
    /// <param name="target">The element to check for.</param>
    /// <returns>True if the element is found, otherwise false.</returns>
    public bool Contains(T target)
    {
        return IndexOf(target) >= 0;
    }

    /// <summary>
    /// Copies the elements of the array to the specified destination array, starting at the specified index in the destination array.
    /// </summary>
    /// <param name="array">The destination array.</param>
    /// <param name="arrayIndex">The index in the destination array to start copying.</param>
    /// <exception cref="ArgumentNullException">Thrown if the destination array is null.</exception>
    /// <exception cref="RankException">Thrown if the destination array is not a one-dimensional array.</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);

        if (array != null && array.Rank != 1)
            throw new RankException($"{typeof(DynArray<T>).Name}.CopyTo expects plain array as an argument");

        Array.Copy(data, 0, array!, arrayIndex, Count);
    }

    /// <summary>
    /// Removes the first occurrence of the specified element from the array.
    /// </summary>
    /// <param name="target">The element to remove.</param>
    /// <returns>True if the element was removed, otherwise false.</returns>
    public bool Remove(T target)
    {
        int index = IndexOf(target);

        if (index >= 0)
        {
            RemoveAt(index);
        }

        return index >= 0;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the array.
    /// </summary>
    /// <returns>An enumerator for the array.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return data[i]!;
        }
    }

    /// <summary>
    /// Returns a non-generic enumerator that iterates through the array.
    /// </summary>
    /// <returns>A non-generic enumerator for the array.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Returns a string representation of the array, with each element separated by a comma.
    /// </summary>
    /// <returns>A string representation of the array.</returns>
    public override string ToString()
    {
        // We don't know what length T.ToString() have.
        var buffer = new StringBuilder();
        if (!IsJagged) buffer.Append('[');

        for (int i = 0; i < Count; i++)
        {
            if (i > 0) buffer.Append(", ");
            if (IsJagged && i > 0) buffer.Append('\n');
            _ = buffer.Append(data[i]?.ToString());
        }

        if (!IsJagged) buffer.Append(']');
        return buffer.ToString();
    }

    /// <summary>
    /// Determines whether the current array is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>True if the arrays are equal, otherwise false.</returns>
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

    /// <summary>
    /// Returns a hash code for the current array.
    /// </summary>
    /// <returns>A hash code for the array.</returns>
    public override int GetHashCode()
    {
        HashCode hash = new();

        foreach (T item in this)
        {
            hash.Add(item);
        }

        return hash.ToHashCode();
    }

    /// <summary>
    /// Creates a copy of the array.
    /// </summary>
    /// <returns>A new `DynArray` with the same elements as the current array.</returns>
    object ICloneable.Clone()
    {
        return this[..Count];
    }

    /// <summary>
    /// Creates a copy of the array.
    /// </summary>
    /// <returns>A new `DynArray` with the same elements as the current array.</returns>
    public DynArray<T> Clone()
    {
        return this[..Count];
    }

    /// <summary>
    /// Determines whether two arrays are equal.
    /// </summary>
    /// <param name="a">The first array.</param>
    /// <param name="b">The second array.</param>
    /// <returns>True if the arrays are equal, otherwise false.</returns>
    public static bool operator ==(DynArray<T>? a, DynArray<T>? b)
    {
        return Equals(a, b);
    }

    /// <summary>
    /// Determines whether two arrays are not equal.
    /// </summary>
    /// <param name="a">The first array.</param>
    /// <param name="b">The second array.</param>
    /// <returns>True if the arrays are not equal, otherwise false.</returns>
    public static bool operator !=(DynArray<T>? a, DynArray<T>? b)
    {
        return !Equals(a, b);
    }

    /// <summary>
    /// Concatenates two arrays and returns a new array containing all the elements of both arrays.
    /// </summary>
    /// <param name="a">The first array.</param>
    /// <param name="b">The second array.</param>
    /// <returns>A new `DynArray` containing the elements of both arrays.</returns>
    public static DynArray<T> operator +(DynArray<T> a, DynArray<T> b)
    {
        if (a.Count == 0) return b;
        if (b.Count == 0) return a;

        var result = new DynArray<T>(length: a.Count + b.Count);
        var i = 0;

        for (int j = 0; j < a.Count; i++, j++)
        {
            result.data[i] = a[j];
        }

        for (int j = 0; j < b.Count; i++, j++)
        {
            result.data[i] = b[j];
        }

        return result;
    }
}
