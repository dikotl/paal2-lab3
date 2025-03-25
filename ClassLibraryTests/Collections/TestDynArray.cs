using System;
using System.Text;
using ClassLibrary.Collections;
using ClassLibrary.FunctionalEnumerableOperations;
using static ClassLibraryTests.Collections.DynArray.ConstValues;

namespace ClassLibraryTests.Collections.DynArray;

[TestClass]
public sealed class Basics
{
    [TestMethod]
    public void TestGetSetIndex()
    {
        for (int i = 0; i < RepeatTime; i++)
        {
            var mainArr = Generator.GetRandomDoubleArray(DefaultMaxSize, DefaultElementBound);
            var dynArr = mainArr.ToDynArray();

            for (int j = 0; j < mainArr.Length; j++)
            {
                Assert.AreEqual(mainArr[j], dynArr[j]);
            }
        }
    }

    [TestMethod]
    public void TestGetCount()
    {
        for (int i = 0; i < RepeatTime; i++)
        {
            var mainArr = Generator.GetRandomDoubleArray(DefaultMaxSize, DefaultElementBound);
            var dynArr = mainArr.ToDynArray();

            Assert.AreEqual(mainArr.Length, dynArr.Count);
        }
    }

    [TestMethod]
    public void TestGetCapacity()
    {
        for (int i = 0; i < RepeatTime; i++)
        {
            var capacity = (int)Generator.Rand.NextInt64(
                    DefaultMaxSize.Start.Value,
                    DefaultMaxSize.End.Value);

            var arr = new DynArray<int>(capacity: capacity);

            Assert.AreEqual(arr.Capacity, capacity);
        }
    }

    [TestMethod]
    public void TestGetEnumerator()
    {
        for (int i = 0; i < RepeatTime; i++)
        {
            var mainArr = Generator.GetRandomDoubleArray(DefaultMaxSize, DefaultElementBound);
            var dynArr = mainArr.ToDynArray();
            var dynEnumerator = dynArr.GetEnumerator();

            foreach (var j in mainArr)
            {
                dynEnumerator.MoveNext();
                Assert.AreEqual(j, dynEnumerator.Current);
            }
        }
    }

    [TestMethod]
    public void TestSlice()
    {
        for (int i = 0; i < RepeatTime / 100 + 1; i++)
        {
            var mainArr = Generator.GetRandomDoubleArray(DefaultMaxSize, DefaultElementBound);
            var dynArr = mainArr.ToDynArray();

            for (int j = 0; j < mainArr.Length; j++)
            {
                var mainSliceForward = mainArr[..j];
                var dynSliceForward = dynArr[..j];
                var mainEnumerator = mainSliceForward.GetEnumerator();

                foreach (var k in dynSliceForward) // GetEnumerator must pass the test
                {
                    mainEnumerator.MoveNext();
                    Assert.AreEqual(k, mainEnumerator.Current);
                }

                var mainSliceBackwards = mainArr[j..];
                var dynSliceBackwards = dynArr[j..];
                var mainEnumerator2 = mainSliceBackwards.GetEnumerator();

                foreach (var k in dynSliceBackwards) // GetEnumerator must pass the test
                {
                    mainEnumerator2.MoveNext();
                    Assert.AreEqual(k, mainEnumerator2.Current);
                }

                for (int j2 = 0; j2 <= j; j2++)
                {
                    var mainSliceDouble = mainArr[j2..j];
                    var dynSliceDouble = dynArr[j2..j];
                    var mainEnumerator3 = mainSliceDouble.GetEnumerator();

                    foreach (var k in dynSliceDouble) // GetEnumerator must pass the test
                    {
                        mainEnumerator3.MoveNext();
                        Assert.AreEqual(k, mainEnumerator3.Current);
                    }
                }
            }
        }
    }
}

[TestClass]
public sealed class ListOps
{
    [TestMethod]
    public void TestIndexOf()
    {
        for (int i = 0; i < RepeatTime; i++)
        {
            var mainArr = Generator
                .GetRandomDoubleArray(DefaultMaxSize, DefaultElementBound)
                .ToList();

            var dynArr = mainArr.ToDynArray();

            for (int j = 0; j < mainArr.Count; j++)
            {
                Assert.AreEqual(
                    mainArr[mainArr.IndexOf(mainArr[j])],
                    dynArr[dynArr.IndexOf(dynArr[j])]);
            }
        }
    }

    [TestMethod]
    public void TestInsert()
    {
        for (int i = 0; i < RepeatTime; i++)
        {
            var mainArr = Generator
                .GetRandomDoubleArray(DefaultMaxSize, DefaultElementBound)
                .ToList();

            var dynArr = mainArr.ToDynArray();

            for (int j = 0; j < RepeatTime / 10 + 1; j++)
            {
                var valToInsert =
                    Generator.Rand.NextDouble() *
                    Generator.Rand.Next(-DefaultElementBound, DefaultElementBound);

                var indexToInsert = Generator.Rand.Next(0, mainArr.Count - 1);

                mainArr.Insert(indexToInsert, valToInsert);
                dynArr.Insert(indexToInsert, valToInsert);

                for (int k = 0; k < mainArr.Count; k++)
                {
                    Assert.AreEqual(mainArr[k], dynArr[k]);
                }
            }
        }
    }

    [TestMethod]
    public void TestRemove()
    {
        for (int i = 0; i < RepeatTime; i++)
        {
            var list = Generator
                .GetRandomIntArray(DefaultMaxSize, DefaultElementBound)
                .ToList();

            var arr = list.ToDynArray();
            var index = Generator.Rand.Next(0, list.Count);

            list.RemoveAt(index);
            arr.RemoveAt(index);
            Assert.AreEqual(list.Count, arr.Count);
        }
    }

    [TestMethod]
    public void TestAdd()
    {
        for (int i = 0; i < RepeatTime; i++)
        {
            var mainArr = Generator
                .GetRandomDoubleArray(DefaultMaxSize, DefaultElementBound)
                .ToList();

            var dynArr = mainArr.ToDynArray();

            for (int j = 0; j < RepeatTime / 10 + 1; j++)
            {
                var valToAdd =
                    Generator.Rand.NextDouble() *
                    Generator.Rand.Next(-DefaultElementBound, DefaultElementBound);

                mainArr.Add(valToAdd);
                dynArr.Add(valToAdd);

                for (int k = 0; k < mainArr.Count; k++)
                {
                    Assert.AreEqual(mainArr[k], dynArr[k]);
                }
            }
        }
    }

    [TestMethod]
    public void TestClear()
    {
        for (int i = 0; i < RepeatTime * 10; i++)
        {
            var dynArr = Generator
                .GetRandomDoubleArray(DefaultMaxSize, DefaultElementBound)
                .ToDynArray();

            dynArr.Clear();
            Assert.AreEqual([], dynArr);
        }
    }

    [TestMethod]
    public void TestContains()
    {
        for (int i = 0; i < RepeatTime / 30 + 1; i++)
        {
            var mainArr = Generator
                .GetRandomIntArray(DefaultMaxSize, DefaultElementBound)
                .ToList();

            var dynArr = mainArr.ToDynArray();
            var mainArrMax = mainArr.Max();

            for (int j = mainArr.Min(); j < mainArrMax; j++)
            {
                Assert.AreEqual(mainArr.Contains(j), dynArr.Contains(j));
            }
        }
    }
}

[TestClass]
public sealed class ResizeOps
{
    [TestMethod]
    public void TestReserve()
    {
    }

    [TestMethod]
    public void TestResize()
    {
        DynArray<int> a = [];

        a.Resize(10);
        Assert.AreEqual(10, a.Count);
    }

    [TestMethod]
    public void TestRemove()
    {
    }
}

[TestClass]
public sealed class Ops
{
    [TestMethod]
    public void TestOpEquals()
    {
        for (int i = 0; i < RepeatTime * 10; i++)
        {

            var dynArr1 = Generator
                .GetRandomDoubleArray(DefaultMaxSize, DefaultElementBound)
                .ToDynArray();

            var mustBeNotSame = Generator.Rand.Next(0, 2) != 0;
            var dynArr2 = mustBeNotSame ? dynArr1.ToDynArray() : dynArr1;

            Assert.AreEqual(mustBeNotSame, dynArr1 != dynArr2);
        }
    }

    [TestMethod]
    public void TestOpPlus()
    {
    }
}

[TestClass]
public sealed class Other
{
    [TestMethod]
    public void TestCopyTo()
    {
        for (int i = 0; i < RepeatTime; i++)
        {
            var source = Generator.GetRandomIntArray(DefaultMaxSize, DefaultElementBound);
            var dyn = source.ToDynArray();
            var copy = new int[source.Length];

            dyn.CopyTo(copy, 0);

            for (int j = 0; j < source.Length; j++)
            {
                Assert.AreEqual(source[j], copy[j]);
            }
        }
    }

    [TestMethod]
    public void TestToString()
    {
        for (int i = 0; i < RepeatTime; i++)
        {
            var dynArr = Generator
                .GetRandomIntArray(DefaultMaxSize, DefaultElementBound)
                .ToDynArray();

            var sb = new StringBuilder().Append('[');
            sb.Append(string.Join(", ", dynArr));
            sb.Append(']');
            Assert.AreEqual(sb.ToString(), dynArr.ToString());
        }
    }

    [TestMethod]
    public void TestEquals()
    {
        for (int i = 0; i < RepeatTime * 10; i++)
        {
            var dynArr1 = Generator
                .GetRandomDoubleArray(DefaultMaxSize, DefaultElementBound)
                .ToDynArray();

            var mustBeNotSame = Generator.Rand.Next(0, 2) != 0;
            var dynArr2 = mustBeNotSame ? dynArr1.ToDynArray() : dynArr1;

            Assert.AreEqual(!mustBeNotSame, dynArr1.Equals(dynArr2));
        }
    }

    [TestMethod]
    public void TestClone()
    {
        for (int i = 0; i < RepeatTime; i++)
        {
            var mainArr = Generator.GetRandomDoubleArray(DefaultMaxSize, DefaultElementBound);
            var dynArr = mainArr.ToDynArray();
            var mainCopy = (double[])mainArr.Clone();
            var dynCopy = dynArr.Clone();
            var mainEnumerator = mainCopy.GetEnumerator();

            foreach (var k in dynCopy) // GetEnumerator must pass the test
            {
                mainEnumerator.MoveNext();
                Assert.AreEqual(k, mainEnumerator.Current);
            }
        }
    }
}


file static class ConstValues
{
    public static readonly Range DefaultMaxSize = ..600;

    public const int RepeatTime = 3000;
    public const int DefaultElementBound = 10000;
}
