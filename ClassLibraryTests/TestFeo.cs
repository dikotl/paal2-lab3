using System;
using FunctionalEnumerableOperations;
using Feo = FunctionalEnumerableOperations;
using static ClassLibraryTests.GlobalRandom;
using System.Numerics;

namespace ClassLibraryTests;

[TestClass]
public sealed class TestFeo
{
    const int repeatTime = 300;
    const int defMaxSize = 600;
    const int defElementBound = 10000;


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
    public void TestMinMax___Int() => TestMinMax(GetIntRandArray);

    [TestMethod]
    public void TestMinMax___Double() => TestMinMax(GetDoubleRandArray);

    [TestMethod]
    public void TestSkip___Int() => TestSkip(GetIntRandArray);

    [TestMethod]
    public void TestSkip___Double() => TestSkip(GetDoubleRandArray);

    [TestMethod]
    public void TestTake___Int() => TestTake(GetIntRandArray);

    [TestMethod]
    public void TestTake___Double() => TestTake(GetDoubleRandArray);

    private void TestMinMax<T>(Func<int, uint, T[]> generateArray) where T : IComparisonOperators<T, T, bool>
    {
        for (var i = 0; i < repeatTime; i++)
        {
            var a = generateArray(defMaxSize, defElementBound);
            var expectedMin = a.Min();
            var expectedMax = a.Max();
            var min = System.Linq.Enumerable.Min(a);
            var max = System.Linq.Enumerable.Max(a);

            Assert.AreEqual(min, expectedMin, $"Feo.Min <{typeof(T)}> does not work correctly");
            Assert.AreEqual(max, expectedMax, $"Feo.Max <{typeof(T)}> does not work correctly");
        }
    }

    private void TestSkip<T>(Func<int, uint, T[]> generateArray)
    {
        for (var i = 0; i < repeatTime; i++)
        {
            var a = generateArray(defMaxSize, defElementBound);
            var skipLen = Rand.Next(a.Length);
            var expected = string.Join(" ", System.Linq.Enumerable.Skip(a, skipLen));
            var actual = string.Join(" ", a.Skip(skipLen));

            Assert.AreEqual(actual, expected, $"Feo.Skip <{typeof(T)}> does not work correctly");
        }
    }

    private void TestTake<T>(Func<int, uint, T[]> generateArray)
    {
        for (var i = 0; i < repeatTime; i++)
        {
            var a = generateArray(defMaxSize, defElementBound);
            var takeLen = Rand.Next(a.Length);
            var expected = string.Join(" ", System.Linq.Enumerable.Take(a, takeLen));
            var actual = string.Join(" ", a.Take(takeLen));

            Assert.AreEqual(actual, expected, $"Feo.Take <{typeof(T)}> does not work correctly");
        }
    }
}

file static class GlobalRandom
{
    public static Random Rand { get; } = new();
}
