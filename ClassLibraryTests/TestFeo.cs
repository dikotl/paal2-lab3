using System;
using System.Numerics;
using System.Reflection;
using System.Linq.Expressions;
using FunctionalEnumerableOperations;
using Feo = FunctionalEnumerableOperations;
using static ClassLibraryTests.GlobalRandom;
using static ClassLibraryTests.ConstValues;

namespace ClassLibraryTests;

[TestClass]
public sealed class FeoTransforms
{
    [TestMethod]
    public void TestSkip___Int() =>
        FeoTestImplementation.TestSkip(ArrayGenerator.GetIntRandArray);

    [TestMethod]
    public void TestSkip___Double() =>
        FeoTestImplementation.TestSkip(ArrayGenerator.GetDoubleRandArray);

    [TestMethod]
    public void TestTake___Int() =>
        FeoTestImplementation.TestTake(ArrayGenerator.GetIntRandArray);

    [TestMethod]
    public void TestTake___Double() =>
        FeoTestImplementation.TestTake(ArrayGenerator.GetDoubleRandArray);
}

[TestClass]
public sealed class FeoBoolAggregations
{
    [TestMethod]
    public void TestAll___Int() =>
        FeoTestImplementation.TestAll(ArrayGenerator.GetIntRandArray,
        [
            a => a < 0,
            a => a > 0,
            a => a == 0,
            a => a % 2 == 0,
        ]);

    [TestMethod]
    public void TestAll___Double() =>
        FeoTestImplementation.TestAll(ArrayGenerator.GetDoubleRandArray,
        [
            a => a < 0,
            a => a > 0,
            a => a == 0,
            a => a - (int)a > 0.5,
        ]);

    [TestMethod]
    public void TestAny___Int() =>
        FeoTestImplementation.TestAny(ArrayGenerator.GetIntRandArray,
        [
            a => a < 0,
            a => a > 0,
            a => a == 0,
            a => a % 2 == 0,
        ]);

    [TestMethod]
    public void TestAny___Double() =>
        FeoTestImplementation.TestAny(ArrayGenerator.GetDoubleRandArray,
        [
            a => a < 0,
            a => a > 0,
            a => a == 0,
            a => a - (int)a > 0.5,
        ]);
}

[TestClass]
public sealed class FeoConvertors
{
    [TestMethod]
    public void TestToArray___Int() =>
        FeoTestImplementation.TestToArray(ArrayGenerator.GetIntRandArray);

    [TestMethod]
    public void TestToArray___Double() =>
        FeoTestImplementation.TestToArray(ArrayGenerator.GetDoubleRandArray);

    [TestMethod]
    public void TestToList___Int() =>
        FeoTestImplementation.TestToList(ArrayGenerator.GetIntRandArray);

    [TestMethod]
    public void TestToList___Double() =>
        FeoTestImplementation.TestToList(ArrayGenerator.GetDoubleRandArray);

    [TestMethod]
    public void TestToSet___Int() =>
        FeoTestImplementation.TestToSet(ArrayGenerator.GetIntRandArray);

    [TestMethod]
    public void TestToSet___Double() =>
        FeoTestImplementation.TestToSet(ArrayGenerator.GetDoubleRandArray);
}

[TestClass]
public sealed class FeoFolds
{

}

[TestClass]
public sealed class ValueFinders
{
    [TestMethod]
    public void TestMinMax___Int() =>
        FeoTestImplementation.TestMinMax(ArrayGenerator.GetIntRandArray);

    [TestMethod]
    public void TestMinMax___Double() =>
        FeoTestImplementation.TestMinMax(ArrayGenerator.GetDoubleRandArray);

    [TestMethod]
    public void TestFirstOrDefault_1___Int() =>
        FeoTestImplementation.TestFirstOrDefault_1(ArrayGenerator.GetIntRandArray,
       [
           a => a < 0,
            a => a > 0,
            a => a % 2 == 0,
            a => a == 0,
            a => a > DefElementBound / 2
       ]);

    [TestMethod]
    public void TestFirstOrDefault_1___Double() =>
        FeoTestImplementation.TestFirstOrDefault_1(ArrayGenerator.GetDoubleRandArray,
        [
            a => a < 0,
            a => a > 0,
            a => a == 0,
            a => a - (int)a > 0.5,
            a => a > DefElementBound / 2
        ]);

    [TestMethod]
    public void TestFirstOrDefault_2___Int() =>
        FeoTestImplementation.TestFirstOrDefault_2(ArrayGenerator.GetIntRandArray,
        [
            a => a < 0,
            a => a > 0,
            a => a % 2 == 0,
            a => a == 0,
            a => a > DefElementBound / 2
        ]);

    [TestMethod]
    public void TestFirstOrDefault_2___Double() =>
        FeoTestImplementation.TestFirstOrDefault_2(ArrayGenerator.GetDoubleRandArray,
        [
            a => a < 0,
            a => a > 0,
            a => a == 0,
            a => a - (int)a > 0.5,
            a => a > DefElementBound / 2
        ]);


}

[TestClass]
public sealed class Sorters
{

}


public static class FeoTestImplementation
{
    public static void TestMinMax<T>(Func<int, uint, T[]> generateArray)
        where T : IComparisonOperators<T, T, bool>
    {
        for (var i = 0; i < RepeatTime; i++)
        {
            var a = generateArray(DefMaxSize, DefElementBound);
            var expectedMin = a.Min();
            var expectedMax = a.Max();
            var min = System.Linq.Enumerable.Min(a);
            var max = System.Linq.Enumerable.Max(a);

            Assert.AreEqual(expectedMin, min, $"Feo.Min <{typeof(T)}> does not work correctly");
            Assert.AreEqual(expectedMax, max, $"Feo.Max <{typeof(T)}> does not work correctly");
        }
    }

    public static void TestSkip<T>(Func<int, uint, T[]> generateArray)
    {
        for (var i = 0; i < RepeatTime; i++)
        {
            var a = generateArray(DefMaxSize, DefElementBound);
            var skipLen = Rand.Next(a.Length);
            var expected = string.Join(" ", System.Linq.Enumerable.Skip(a, skipLen));
            var actual = string.Join(" ", a.Skip(skipLen));

            Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} " +
            $"<{typeof(T)}> test target does not work correctly");
        }
    }

    public static void TestTake<T>(Func<int, uint, T[]> generateArray)
    {
        for (var i = 0; i < RepeatTime; i++)
        {
            var a = generateArray(DefMaxSize, DefElementBound);
            var takeLen = Rand.Next(a.Length);
            var expected = string.Join(" ", System.Linq.Enumerable.Take(a, takeLen));
            var actual = string.Join(" ", a.Take(takeLen));

            Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} " +
            $"<{typeof(T)}> test target does not work correctly");
        }
    }

    public static void TestFirstOrDefault_1<T>(Func<int, uint, T[]> generateArray, Expression<Func<T, bool>>[] predicates)
    {
        foreach (var predicate in predicates)
            for (var i = 0; i < RepeatTime / predicates.Length; i++)
            {
                var a = generateArray(DefMaxSize, DefElementBound);
                var expected = string.Join(" ",
                    System.Linq.Enumerable.FirstOrDefault(a, predicate.Compile()));
                var actual = string.Join(" ", a.FirstOrDefault(predicate.Compile()));

                Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} " +
                $"<{typeof(T)}> test target does not work correctly with {predicate.Body}");
            }
    }

    public static void TestFirstOrDefault_2<T>(Func<int, uint, T[]> generateArray, Expression<Func<T, bool>>[] predicates)
        where T : IAdditionOperators<T, T, T>,
                  IDivisionOperators<T, T, T>
    {
        foreach (var predicate in predicates)
            for (var i = 0; i < RepeatTime / predicates.Length; i++)
            {
                var a = generateArray(DefMaxSize, DefElementBound);
                T defaultValue =
                    System.Linq.Enumerable.Aggregate(a, (a, b) => a + b) /
                    (T)Convert.ChangeType(a.Length, typeof(T));
                var expected = string.Join(" ",
                    System.Linq.Enumerable.FirstOrDefault(a, predicate.Compile(), defaultValue));
                var actual = string.Join(" ", a.FirstOrDefault(predicate.Compile(), defaultValue));

                Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} " +
                $"<{typeof(T)}> test target does not work correctly with {predicate.Body}");
            }
    }

    public static void TestAll<T>(Func<int, uint, T[]> generateArray, Expression<Func<T, bool>>[] predicates)
    {
        foreach (var predicate in predicates)
            for (var i = 0; i < RepeatTime / predicates.Length * 20; i++)
            {
                var a = generateArray(15, 20);
                var expected = string.Join(" ", System.Linq.Enumerable.All(a, predicate.Compile()));
                var actual = string.Join(" ", a.All(predicate.Compile()));

                Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} " +
                $"<{typeof(T)}> test target does not work correctly with {predicate.Body}");
            }
    }

    public static void TestAny<T>(Func<int, uint, T[]> generateArray, Expression<Func<T, bool>>[] predicates)
    {
        foreach (var predicate in predicates)
            for (var i = 0; i < RepeatTime / predicates.Length * 70; i++)
            {
                var a = generateArray(15, 20);
                var expected = string.Join(" ", System.Linq.Enumerable.All(a, predicate.Compile()));
                var actual = string.Join(" ", a.All(predicate.Compile()));

                Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} " +
                $"<{typeof(T)}> test target does not work correctly with {predicate.Body}");
            }
    }

    public static void TestToArray<T>(Func<int, uint, T[]> generateArray)
    {
        for (var i = 0; i < RepeatTime; i++)
        {
            var a = generateArray(DefMaxSize * 3, DefElementBound * 2);
            var expected = string.Join(" ", System.Linq.Enumerable.ToArray(a));
            var actual = string.Join(" ", a.ToArray());

            Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} " +
            $"<{typeof(T)}> test target does not work correctly");
        }
    }

    public static void TestToList<T>(Func<int, uint, T[]> generateArray)
    {
        for (var i = 0; i < RepeatTime; i++)
        {
            var a = generateArray(DefMaxSize * 3, DefElementBound * 2);
            var expected = string.Join(" ", System.Linq.Enumerable.ToList(a));
            var actual = string.Join(" ", a.ToList());

            Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} " +
            $"<{typeof(T)}> test target does not work correctly");
        }
    }

    public static void TestToSet<T>(Func<int, uint, T[]> generateArray)
    {
        for (var i = 0; i < RepeatTime; i++)
        {
            var a = generateArray(DefMaxSize * 3, DefElementBound * 2);
            var expected = string.Join(" ", System.Linq.Enumerable.ToHashSet(a));
            var actual = string.Join(" ", a.ToSet());

            Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} " +
            $"<{typeof(T)}> test target does not work correctly");
        }
    }
}


file static class ArrayGenerator
{
    public static int[] GetIntRandArray(int maxSize, uint maxElementBound)
    {
        int size = Rand.Next(1, maxSize + 1);
        int[] array = new int[size];

        for (int i = 0; i < size; i++)
            array[i] = Rand.Next(-(int)maxElementBound, (int)maxElementBound + 1);

        return array;
    }
    public static double[] GetDoubleRandArray(int maxSize, uint maxElementBound)
    {
        int size = Rand.Next(1, maxSize + 1);
        double[] array = new double[size];

        for (int i = 0; i < size; i++)
            array[i] = Rand.Next(-(int)maxElementBound,
                                (int)maxElementBound + 1) * Rand.NextDouble();

        return array;
    }
}

file static class ConstValues
{
    public const int RepeatTime = 300;
    public const int DefMaxSize = 600;
    public const int DefElementBound = 10000;
}

file static class GlobalRandom
{
    public static Random Rand { get; } = new();
}
