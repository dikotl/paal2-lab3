using System;
using System.Numerics;
using System.Reflection;
using System.Linq.Expressions;
using FunctionalEnumerableOperations;
using Feo = FunctionalEnumerableOperations;
using static ClassLibraryTests.GlobalRandom;
using static ClassLibraryTests.ConstValues;
using System.Text;
using System.Collections.Generic;

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

    [TestMethod]
    public void TestFilter_1___Int() =>
        FeoTestImplementation.TestFilter_1(ArrayGenerator.GetIntRandArray, [
            a => a < 0,
            a => a > 0,
            a => a % 2 == 0,
            a => a > DefElementBound / 2,
            a => a.ToString().Contains("7"),
            a => Math.Sin(a) > 0,
            a => Math.Cos(a) < 0,
            a => Math.Log(a) > 1,
            a => a > 0 && (a & 1) == 1,
            a => a % 5 == 0,
            a => a > 0 && Math.Sqrt(a) % 1 == 0,
        ]);

    [TestMethod]
    public void TestFilter_1___Double() =>
        FeoTestImplementation.TestFilter_1(ArrayGenerator.GetDoubleRandArray, [
            a => a < 0,
            a => a > 0,
            a => a % 2 == 0,
            a => a > DefElementBound / 2,
            a => Math.Abs(a) < 1e-10,
            a => a % 1 == 0,
            a => a.ToString().Contains("7"),
            a => Math.Sin(a) > 0,
            a => Math.Cos(a) < 0,
            a => Math.Log(a) > 1,
            a => a % 5 == 0,
        ]);

    [TestMethod]
    public void TestFilter_2___Int() =>
        FeoTestImplementation.TestFilter_2(ArrayGenerator.GetIntRandArray, [
            (a, i) => a % 2 == 0 && i % 2 == 0,
            (a, i) => a % 2 != 0 && i % 2 != 0,
            (a, i) => a == i,
            (a, i) => a > i,
            (a, i) => a < i,
            (a, i) => (a + i) % 10 == 7,
            (a, i) => (a & i) == 0,
            (a, i) => (a | i) % 2 == 1,
            (a, i) => i != 0 && a % i == 0,
            (a, i) => i > 0 && a % i == i % 3,
        ]);

    [TestMethod]
    public void TestFilter_2___Double() =>
        FeoTestImplementation.TestFilter_2(ArrayGenerator.GetDoubleRandArray, [
            (a, i) => a > 0 && i % 2 == 0,
            (a, i) => a < 0 && i % 2 != 0,
            (a, i) => Math.Abs(a) < i,
            (a, i) => a > i * 2,
            (a, i) => Math.Floor(a) == i,
            (a, i) => i > 0 && Math.Pow(a, 1.0 / i) % 1 == 0,
            (a, i) => i != 0 && a % i < 0.5,
            (a, i) => double.IsNaN(a) || i == 0,
            (a, i) => Math.Sin(a + i) > 0,
            (a, i) => a > 0 && Math.Log(a + i) > 1,
        ]);

    [TestMethod]
    public void TestTakeWhile___Int() =>
        FeoTestImplementation.TestTakeWhile(ArrayGenerator.GetIntRandArray, [
            a => a > 0,
            a => a % 2 == 0,
            a => a < 100,
            a => a != 0,
            a => a % 10 != 7,
            a => (a & 1) == 0,
            a => a > -50 && a < 50,
            a => a * a > 10000,
        ]);


    [TestMethod]
    public void TestTakeWhile___Double() =>
        FeoTestImplementation.TestTakeWhile(ArrayGenerator.GetDoubleRandArray, [
            a => a > 0,
            a => a < 100,
            a => Math.Abs(a) > 0.1,
            a => a % 1 != 0,
            a => Math.Sin(a) >= 0,
            a => Math.Log(Math.Abs(a) + 1) < 2,
            a => a * a < 10000,
        ]);

    [TestMethod]
    public void TestSkipWhile___Int() =>
        FeoTestImplementation.TestSkipWhile(ArrayGenerator.GetIntRandArray, [
            a => a > 0,
            a => a % 2 == 0,
            a => a < 100,
            a => a != 0,
            a => a % 10 != 7,
            a => (a & 1) == 0,
            a => a > -50 && a < 50,
            a => a * a > 10000,
        ]);


    [TestMethod]
    public void TestSkipWhile___Double() =>
        FeoTestImplementation.TestSkipWhile(ArrayGenerator.GetDoubleRandArray, [
            a => a > 0,
            a => a < 100,
            a => Math.Abs(a) > 0.1,
            a => a % 1 != 0,
            a => Math.Sin(a) >= 0,
            a => Math.Log(Math.Abs(a) + 1) < 2,
            a => a * a < 10000,
        ]);

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
    [TestMethod]
    public void TestFold_1___Int() =>
        FeoTestImplementation.TestFold_1<int>(ArrayGenerator.GetIntRandArray,
        [
            (acc, c) => acc + c,
            (acc, c) => c < acc ? c : acc,
            (acc, c) => c > acc ? c : acc,
            (acc, c) => acc ^ c,
            (acc, c) => acc * (c % 10 == 0 ? 1 : c),
            (acc, c) => acc + (c % 2 == 0 ? c : -c),
            (acc, c) => (acc * 31 + c) % 1000000007,
        ]);

    [TestMethod]
    public void TestFold_1___Double() =>
        FeoTestImplementation.TestFold_1<double>(ArrayGenerator.GetDoubleRandArray,
        [
            (acc, c) => acc + c,
            (acc, c) => c < acc ? c : acc,
            (acc, c) => c > acc ? c : acc,
            (acc, c) => acc + Math.Sin(c),
            (acc, c) => acc * Math.Exp(-Math.Abs(c)),
            (acc, c) => acc + (c > 0 ? c : 0),
            (acc, c) => Math.Max(acc, Math.Abs(c)),
        ]);


    [TestMethod]
    public void TestFold_2___Int() =>
        FeoTestImplementation.TestFold_2(ArrayGenerator.GetIntRandArray,
        Rand.Next(DefElementBound),
        [
            (acc, c) => acc + c,
            (acc, c) => c < acc ? c : acc,
            (acc, c) => c > acc ? c : acc,
            (acc, c) => acc ^ c,
            (acc, c) => acc * (c % 10 == 0 ? 1 : c),
            (acc, c) => acc + (c % 2 == 0 ? c : -c),
            (acc, c) => (acc * 31 + c) % 1000000007,
        ]);

    [TestMethod]
    public void TestFold_2___Double() =>
        FeoTestImplementation.TestFold_2(ArrayGenerator.GetDoubleRandArray,
        Rand.Next(DefElementBound) * Rand.NextDouble(),
        [
            (acc, c) => acc + c,
            (acc, c) => c < acc ? c : acc,
            (acc, c) => c > acc ? c : acc,
            (acc, c) => acc + Math.Sin(c),
            (acc, c) => acc * Math.Exp(-Math.Abs(c)),
            (acc, c) => acc + (c > 0 ? c : 0),
            (acc, c) => Math.Max(acc, Math.Abs(c)),
        ]);

    [TestMethod]
    public void TestFold_2___String() =>
        FeoTestImplementation.TestFold_2(ArrayGenerator.GetRandString,
        String.Join("", ArrayGenerator.GetRandString(5, 0)),
        [
            (acc, c) => $"{c}{acc}",
            (acc, c) => $"{acc}{int.Clamp(c + 1, 32, 126)}",
            (acc, c) => $"{acc}{c}{acc}",
            (acc, c) => acc.Contains(c) ? acc : acc + c,
        ]);
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
    public static void TestMinMax<T>(Func<int, uint, IEnumerable<T>> generateArray)
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

    public static void TestSkip<T>(Func<int, uint, IEnumerable<T>> generateArray)
    {
        for (var i = 0; i < RepeatTime; i++)
        {
            var a = generateArray(DefMaxSize, DefElementBound);
            var skipLen = Rand.Next(System.Linq.Enumerable.ToArray(a).Length);
            var expected = string.Join(" ", System.Linq.Enumerable.Skip(a, skipLen));
            var actual = string.Join(" ", a.Skip(skipLen));

            Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} " +
            $"<{typeof(T)}> test target does not work correctly");
        }
    }

    public static void TestTake<T>(Func<int, uint, IEnumerable<T>> generateArray)
    {
        for (var i = 0; i < RepeatTime; i++)
        {
            var a = generateArray(DefMaxSize, DefElementBound);
            var takeLen = Rand.Next(System.Linq.Enumerable.ToArray(a).Length);
            var expected = string.Join(" ", System.Linq.Enumerable.Take(a, takeLen));
            var actual = string.Join(" ", a.Take(takeLen));

            Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} " +
            $"<{typeof(T)}> test target does not work correctly");
        }
    }

    public static void TestFirstOrDefault_1<T>(Func<int, uint, IEnumerable<T>> generateArray, Expression<Func<T, bool>>[] predicates)
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

    public static void TestFirstOrDefault_2<T>(Func<int, uint, IEnumerable<T>> generateArray, Expression<Func<T, bool>>[] predicates)
        where T : IAdditionOperators<T, T, T>,
                  IDivisionOperators<T, T, T>
    {
        foreach (var predicate in predicates)
            for (var i = 0; i < RepeatTime / predicates.Length; i++)
            {
                var a = generateArray(DefMaxSize, DefElementBound);
                T defaultValue =
                    System.Linq.Enumerable.Aggregate(a, (a, b) => a + b) /
                    (T)Convert.ChangeType(System.Linq.Enumerable.ToArray(a).Length, typeof(T));
                var expected = string.Join(" ",
                    System.Linq.Enumerable.FirstOrDefault(a, predicate.Compile(), defaultValue));
                var actual = string.Join(" ", a.FirstOrDefault(predicate.Compile(), defaultValue));

                Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} " +
                $"<{typeof(T)}> test target does not work correctly with {predicate.Body}");
            }
    }

    public static void TestAll<T>(Func<int, uint, IEnumerable<T>> generateArray, Expression<Func<T, bool>>[] predicates)
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

    public static void TestAny<T>(Func<int, uint, IEnumerable<T>> generateArray, Expression<Func<T, bool>>[] predicates)
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

    public static void TestToArray<T>(Func<int, uint, IEnumerable<T>> generateArray)
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

    public static void TestToList<T>(Func<int, uint, IEnumerable<T>> generateArray)
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

    public static void TestToSet<T>(Func<int, uint, IEnumerable<T>> generateArray)
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

    public static void TestFold_1<T>(Func<int, uint, IEnumerable<T>> generateArray, Expression<Func<T, T, T>>[] predicates)
    {
        foreach (var predicate in predicates)
            for (var i = 0; i < RepeatTime / predicates.Length; i++)
            {
                var a = generateArray(DefMaxSize, DefElementBound);
                var expected = string.Join(" ",
                    System.Linq.Enumerable.Aggregate(a, predicate.Compile()));
                var actual = string.Join(" ", a.Fold(predicate.Compile()));

                Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} " +
                $"<{typeof(T)}> test target does not work correctly with {predicate.Body}");
            }
    }

    public static void TestFold_2<T, TAcc>(Func<int, uint, IEnumerable<T>> generateArray, TAcc defVal, Expression<Func<TAcc, T, TAcc>>[] predicates)

    {
        foreach (var predicate in predicates)
            for (var i = 0; i < RepeatTime / predicates.Length; i++)
            {
                var a = generateArray(typeof(TAcc) == typeof(string) ? 15 : DefMaxSize, DefElementBound);
                var expected = string.Join(" ",
                    System.Linq.Enumerable.Aggregate(a, defVal, predicate.Compile()));
                var actual = string.Join(" ", a.Fold(defVal, predicate.Compile()));

                Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} " +
                $"<{typeof(T)}> test target does not work correctly with {predicate.Body}");
            }
    }

    public static void TestFilter_1<T>(Func<int, uint, IEnumerable<T>> generateArray, Expression<Func<T, bool>>[] predicates)
    {
        foreach (var predicate in predicates)
            for (var i = 0; i < RepeatTime / predicates.Length; i++)
            {
                var a = generateArray(DefMaxSize, DefElementBound);
                var expected = string.Join(" ",
                    System.Linq.Enumerable.Where(a, predicate.Compile()));
                var actual = string.Join(" ", a.Filter(predicate.Compile()));

                Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} " +
                $"<{typeof(T)}> test target does not work correctly with {predicate.Body}");
            }
    }

    public static void TestFilter_2<T>(Func<int, uint, IEnumerable<T>> generateArray, Expression<Func<T, int, bool>>[] predicates)
    {
        foreach (var predicate in predicates)
            for (var i = 0; i < RepeatTime / predicates.Length; i++)
            {
                var a = generateArray(DefMaxSize, DefElementBound);
                var expected = string.Join(" ",
                    System.Linq.Enumerable.Where(a, predicate.Compile()));
                var actual = string.Join(" ", a.Filter(predicate.Compile()));

                Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} " +
                $"<{typeof(T)}> test target does not work correctly with {predicate.Body}");
            }
    }

    public static void TestTakeWhile<T>(Func<int, uint, IEnumerable<T>> generateArray, Expression<Func<T, bool>>[] predicates)
    {
        foreach (var predicate in predicates)
            for (var i = 0; i < RepeatTime / predicates.Length; i++)
            {
                var a = generateArray(DefMaxSize, DefElementBound);
                var expected = string.Join(" ",
                    System.Linq.Enumerable.TakeWhile(a, predicate.Compile()));
                var actual = string.Join(" ", a.TakeWhile(predicate.Compile()));

                Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} " +
                $"<{typeof(T)}> test target does not work correctly with {predicate.Body}");
            }
    }

    public static void TestSkipWhile<T>(Func<int, uint, IEnumerable<T>> generateArray, Expression<Func<T, bool>>[] predicates)
    {
        foreach (var predicate in predicates)
            for (var i = 0; i < RepeatTime / predicates.Length; i++)
            {
                var a = generateArray(DefMaxSize, DefElementBound);
                var expected = string.Join(" ",
                    System.Linq.Enumerable.SkipWhile(a, predicate.Compile()));
                var actual = string.Join(" ", a.SkipWhile(predicate.Compile()));

                Assert.AreEqual(expected, actual, $"{MethodBase.GetCurrentMethod()?.Name} " +
                $"<{typeof(T)}> test target does not work correctly with {predicate.Body}");
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

    public static IEnumerable<char> GetRandString(int maxSize, uint signatureNecesary)
    {
        int size = Rand.Next(1, maxSize + 1);
        StringBuilder array = new StringBuilder();

        for (int i = 0; i < size; i++)
            array.Append((char)Rand.Next(32, 127));

        return array.ToString();
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
