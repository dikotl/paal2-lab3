using System;

namespace ClassLibraryCS.Collections;

/// <summary>
/// A utility class that generates random data, including arrays and dynamic arrays.
/// </summary>
public static class Generator
{
    public static readonly Random Rand = new();

    /// <summary>
    /// Generates a random `DynArray` of a random size within the given range and populates it with items generated by the provided function.
    /// </summary>
    /// <typeparam name="T">The type of the items in the array.</typeparam>
    /// <param name="sizeRange">A range specifying the minimum and maximum size of the array.</param>
    /// <param name="getItem">A function that generates items for the array.</param>
    /// <returns>A <c>DynArray&lt;T&gt;</c> containing random items.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the `sizeRange` is not valid (e.g., if `sizeRange.Start.Value` is greater than `sizeRange.End.Value`).</exception>
    public static DynArray<T> GetRandomDynArray<T>(Range sizeRange, Func<T> getItem)
    {
        var size = Rand.Next(int.Max(1, sizeRange.Start.Value), sizeRange.End.Value);
        return new(size, getItem);
    }

    /// <summary>
    /// Generates a random array of type `T` with a random size within the given range and populates it with items generated by the provided function.
    /// </summary>
    /// <typeparam name="T">The type of the items in the array.</typeparam>
    /// <param name="sizeRange">A range specifying the minimum and maximum size of the array.</param>
    /// <param name="getItem">A function that generates items for the array.</param>
    /// <returns>An array of random items.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the `sizeRange` is not valid (e.g., if `sizeRange.Start.Value` is greater than `sizeRange.End.Value`).</exception>
    public static T[] GetRandomArray<T>(Range sizeRange, Func<T> getItem)
    {
        var size = Rand.Next(int.Max(1, sizeRange.Start.Value), sizeRange.End.Value);
        var array = new T[size];

        for (int i = 0; i < size; i++)
            array[i] = getItem();

        return array;
    }

    /// <summary>
    /// Generates a random integer array with a random size within the given range, with values in the range from -maxAbs to maxAbs.
    /// </summary>
    /// <param name="sizeRange">A range specifying the minimum and maximum size of the array.</param>
    /// <param name="maxAbs">The maximum absolute value for each integer in the array.</param>
    /// <returns>An array of random integers.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the `sizeRange` is not valid.</exception>
    public static int[] GetRandomIntArray(Range sizeRange, int maxAbs)
    {
        return GetRandomArray(sizeRange, () => Rand.Next(-maxAbs, maxAbs));
    }

    /// <summary>
    /// Generates a random double array with a random size within the given range, with values in the range from -maxAbs to maxAbs.
    /// </summary>
    /// <param name="sizeRange">A range specifying the minimum and maximum size of the array.</param>
    /// <param name="maxAbs">The maximum absolute value for each double in the array.</param>
    /// <returns>An array of random doubles.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the `sizeRange` is not valid.</exception>
    public static double[] GetRandomDoubleArray(Range sizeRange, int maxAbs)
    {
        return GetRandomArray(sizeRange, () => Rand.NextDouble() * Rand.Next(-maxAbs, maxAbs + 1));
    }
}
