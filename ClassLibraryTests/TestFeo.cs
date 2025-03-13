using System;
using System.Numerics;
using System.Reflection;
using System.Linq.Expressions;
using FunctionalEnumerableOperations;
using Feo = FunctionalEnumerableOperations;
using static ClassLibraryTests.GlobalRandom;

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

    [TestMethod]
    public void TestFirstOrDefault_1___Int() => TestFirstOrDefault_1(GetIntRandArray,
        [
            a => a < 0,
            a => a > 0,
            a => a % 2 == 0,
            a => a == 0,
            a => a > defElementBound / 2
        ]);

    [TestMethod]
    public void TestFirstOrDefault_1___Double() => TestFirstOrDefault_1(GetDoubleRandArray,
        [
            a => a < 0,
            a => a > 0,
            a => a == 0,
            a => a - (int)a > 0.5,
            a => a > defElementBound / 2
        ]);

    [TestMethod]
    public void TestFirstOrDefault_2___Int() => TestFirstOrDefault_2(GetIntRandArray,
        [
            a => a < 0,
            a => a > 0,
            a => a % 2 == 0,
            a => a == 0,
            a => a > defElementBound / 2
        ]);

    [TestMethod]
    public void TestFirstOrDefault_2___Double() => TestFirstOrDefault_2(GetDoubleRandArray,
        [
            a => a < 0,
            a => a > 0,
            a => a == 0,
            a => a - (int)a > 0.5,
            a => a > defElementBound / 2
        ]);


    private void TestMinMax<T>(Func<int, uint, T[]> generateArray) where T : IComparisonOperators<T, T, bool>
    {
        for (var i = 0; i < repeatTime; i++)
        {
            var a = generateArray(defMaxSize, defElementBound);
            var expectedMin = a.Min();
            var expectedMax = a.Max();
            var min = System.Linq.Enumerable.Min(a);
            var max = System.Linq.Enumerable.Max(a);

            Assert.AreEqual(expectedMin, min, $"Feo.Min <{typeof(T)}> does not work correctly");
            Assert.AreEqual(expectedMax, max, $"Feo.Max <{typeof(T)}> does not work correctly");
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

            Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} <{typeof(T)}> test target does not work correctly");
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

            Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} <{typeof(T)}> test target does not work correctly");
        }
    }

    private void TestFirstOrDefault_1<T>(Func<int, uint, T[]> generateArray, Expression<Func<T, bool>>[] predicates)
    {
        foreach (var predicate in predicates)
            for (var i = 0; i < repeatTime / predicates.Length; i++)
            {
                var a = generateArray(defMaxSize, defElementBound);
                var expected = string.Join(" ", System.Linq.Enumerable.FirstOrDefault(a, predicate.Compile()));
                var actual = string.Join(" ", a.FirstOrDefault(predicate.Compile()));

                Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} <{typeof(T)}> test target does not work correctly with {predicate.Body}");
            }
    }

    private void TestFirstOrDefault_2<T>(Func<int, uint, T[]> generateArray, Expression<Func<T, bool>>[] predicates)
        where T : IAdditionOperators<T, T, T>,
                  IDivisionOperators<T, T, T>
    {
        foreach (var predicate in predicates)
            for (var i = 0; i < repeatTime / predicates.Length; i++)
            {
                var a = generateArray(defMaxSize, defElementBound);
                T defaultValue = System.Linq.Enumerable.Aggregate(a, (a, b) => a + b) / (T)Convert.ChangeType(a.Length, typeof(T));
                var expected = string.Join(" ", System.Linq.Enumerable.FirstOrDefault(a, predicate.Compile(), defaultValue));
                var actual = string.Join(" ", a.FirstOrDefault(predicate.Compile(), defaultValue));

                Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} <{typeof(T)}> test target does not work correctly with {predicate.Body}");
            }
    }
}

file static class GlobalRandom
{
    public static Random Rand { get; } = new();
}
