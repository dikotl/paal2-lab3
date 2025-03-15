using System;

namespace ClassLibrary.Collections;

public static class Generator
{
    public static readonly Random Rand = new();

    public static DynArray<T> GetRandomDynArray<T>(Range sizeRange, Func<T> getItem)
    {
        var size = Rand.Next(int.Max(1, sizeRange.Start.Value), sizeRange.End.Value);
        return new(size, getItem);
    }

    public static T[] GetRandomArray<T>(Range sizeRange, Func<T> getItem)
    {
        var size = Rand.Next(int.Max(1, sizeRange.Start.Value), sizeRange.End.Value);
        var array = new T[size];

        for (int i = 0; i < size; i++)
            array[i] = getItem();

        return array;
    }

    public static int[] GetRandomIntArray(Range sizeRange, int maxAbs)
    {
        return GetRandomArray(sizeRange, () => Rand.Next(-maxAbs, maxAbs));
    }

    public static double[] GetRandomDoubleArray(Range sizeRange, int maxAbs)
    {
        return GetRandomArray(sizeRange, () => Rand.NextDouble() * Rand.Next(-maxAbs, maxAbs + 1));
    }
}
