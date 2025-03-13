using System;
using FunctionalEnumerableOperations;
using Feo = FunctionalEnumerableOperations;
using static ClassLibraryTests.GlobalRandom;

namespace ClassLibraryTests;

[TestClass]
public sealed class TestFeo
{

    private int[] GetIntRandArray(int maxSize, uint maxElementBound)
    {
        int size = Rand.Next(1, maxSize + 1);
        int[] array = new int[size];

        for (int i = 0; i < size; i++)
            array[i] = Rand.Next(-(int)maxElementBound, (int)maxElementBound + 1);

        return array;
    }
    private double[] GetDoubleRandArray(int maxSize, uint maxElementBound)
    {
        int size = Rand.Next(1, maxSize + 1);
        double[] array = new double[size];

        for (int i = 0; i < size; i++)
            array[i] = Rand.Next(-(int)maxElementBound, (int)maxElementBound + 1) * Rand.NextDouble();

        return array;
    }


    [TestMethod]
    public void TestMinMaxInt()
    {
        var a = GetIntRandArray(600, 10000);
        var expectedMin = System.Linq.Enumerable.Min(a);
        var expectedMax = System.Linq.Enumerable.Max(a);
        var min = a.Min();
        var max = a.Max();

        Assert.AreEqual(min, expectedMin, "Feo.Min <int> does not work correctly");
        Assert.AreEqual(max, expectedMax, "Feo.Max <int> does not work correctly");
    }
    [TestMethod]
    public void TestMinMaxDouble()
    {
        var a = GetDoubleRandArray(600, 10000);
        var expectedMin = System.Linq.Enumerable.Min(a);
        var expectedMax = System.Linq.Enumerable.Max(a);
        var min = a.Min();
        var max = a.Max();

        Assert.AreEqual(min, expectedMin, "Feo.Min <double> does not work correctly");
        Assert.AreEqual(max, expectedMax, "Feo.Max <double> does not work correctly");
    }

    [TestMethod]
    public void TestSkipInt()
    {
        var a = GetIntRandArray(600, 10000);
        var skipLen = Rand.Next(a.Length);
        var expected = String.Join(" ", System.Linq.Enumerable.Skip(a, skipLen));
        var actual = String.Join(" ", a.Skip(skipLen));

        Assert.AreEqual(actual, expected, "Feo.Skip <int> does not work correctly");
    }

    [TestMethod]
    public void TestSkipDouble()
    {
        var a = GetIntRandArray(600, 10000);
        var skipLen = Rand.Next(a.Length);
        var expected = String.Join(" ", System.Linq.Enumerable.Skip(a, skipLen));
        var actual = String.Join(" ", a.Skip(skipLen));

        Assert.AreEqual(actual, expected, "Feo.Skip <double> does not work correctly");
    }
}

file static class GlobalRandom
{
    public static Random Rand { get; } = new();
}
