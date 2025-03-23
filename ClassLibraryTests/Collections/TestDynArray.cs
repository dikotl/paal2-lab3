using System;
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
            var mainArr = Generator.GetRandomDoubleArray(DefMaxSize, DefElementBound);
            DynArray<double> dynArr = mainArr.ToDynArray();
            for (int j = 0; j < mainArr.Length; j++)
                Assert.AreEqual(mainArr[j], dynArr[j]);
        }
    }

    [TestMethod]
    public void TestGetCount()
    {
        for (int i = 0; i < RepeatTime; i++)
        {
            var mainArr = Generator.GetRandomDoubleArray(DefMaxSize, DefElementBound);
            DynArray<double> dynArr = mainArr.ToDynArray();
            Assert.AreEqual(mainArr.Length, dynArr.Count);
        }
    }

    [TestMethod]
    public void TestGetCapacity()
    {
    }

    [TestMethod]
    public void TestGetEnumerator()
    {
        for (int i = 0; i < RepeatTime; i++)
        {
            var mainArr = Generator.GetRandomDoubleArray(DefMaxSize, DefElementBound);
            DynArray<double> dynArr = mainArr.ToDynArray();
            var dynEnumerator = dynArr.GetEnumerator();

            foreach (var j in mainArr)
            {
                dynEnumerator.MoveNext();
                Assert.AreEqual(j, dynEnumerator.Current);
            }
        }
    }

    [TestMethod]
    public void TestCtors()
    {
    }

    [TestMethod]
    public void TestSlice()
    {
        for (int i = 0; i < RepeatTime / 100 + 1; i++)
        {
            var mainArr = Generator.GetRandomDoubleArray(DefMaxSize, DefElementBound);
            DynArray<double> dynArr = mainArr.ToDynArray();
            for (int j = 0; j < mainArr.Length; j++)
            {
                var mainSliceForward = mainArr[..j];
                var dynSliceForward = dynArr[..j];
                var mainEnumerator = mainSliceForward.GetEnumerator();
                foreach (var k in dynSliceForward)          // GetEnumerator must pass the test
                {
                    mainEnumerator.MoveNext();
                    Assert.AreEqual(k, mainEnumerator.Current);
                }

                var mainSliceBackwards = mainArr[j..];
                var dynSliceBackwards = dynArr[j..];
                var mainEnumerator2 = mainSliceBackwards.GetEnumerator();
                foreach (var k in dynSliceBackwards)         // GetEnumerator must pass the test
                {
                    mainEnumerator2.MoveNext();
                    Assert.AreEqual(k, mainEnumerator2.Current);
                }

                for (int j2 = 0; j2 <= j; j2++)
                {
                    var mainSliceDouble = mainArr[j2..j];
                    var dynSliceDouble = dynArr[j2..j];
                    var mainEnumerator3 = mainSliceDouble.GetEnumerator();
                    foreach (var k in dynSliceDouble)       // GetEnumerator must pass the test
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
            var mainArr = Generator.GetRandomDoubleArray(DefMaxSize, DefElementBound).ToList();
            DynArray<double> dynArr = mainArr.ToDynArray();
            for (int j = 0; j < mainArr.Count; j++)
                Assert.AreEqual(mainArr[mainArr.IndexOf(mainArr[j])], dynArr[dynArr.IndexOf(dynArr[j])]);
        }
    }

    [TestMethod]
    public void TestInsert()
    {
        for (int i = 0; i < RepeatTime; i++)
        {
            var mainArr = Generator.GetRandomDoubleArray(DefMaxSize, DefElementBound).ToList();
            DynArray<double> dynArr = mainArr.ToDynArray();
            for (int j = 0; j < RepeatTime / 10 + 1; j++)
            {
                var valToInsert =
                    Generator.Rand.NextDouble() *
                    Generator.Rand.Next(-DefElementBound, DefElementBound);

                var indexToInsert = Generator.Rand.Next(0, mainArr.Count - 1);

                mainArr.Insert(indexToInsert, valToInsert);
                dynArr.Insert(indexToInsert, valToInsert);

                for (int k = 0; k < mainArr.Count; k++)
                    Assert.AreEqual(mainArr[k], dynArr[k]);
            }
        }
    }

    [TestMethod]
    public void TestRemove()
    {
    }

    [TestMethod]
    public void TestAdd()
    {
        for (int i = 0; i < RepeatTime; i++)
        {
            var mainArr = Generator.GetRandomDoubleArray(DefMaxSize, DefElementBound).ToList();
            DynArray<double> dynArr = mainArr.ToDynArray();
            for (int j = 0; j < RepeatTime / 10 + 1; j++)
            {
                var valToAdd =
                    Generator.Rand.NextDouble() *
                    Generator.Rand.Next(-DefElementBound, DefElementBound);


                mainArr.Add(valToAdd);
                dynArr.Add(valToAdd);

                for (int k = 0; k < mainArr.Count; k++)
                    Assert.AreEqual(mainArr[k], dynArr[k]);
            }
        }
    }

    [TestMethod]
    public void TestClear()
    {
        for (int i = 0; i < RepeatTime * 10; i++)
        {

            DynArray<double> dynArr = Generator.GetRandomDoubleArray(DefMaxSize, DefElementBound).ToDynArray();
            dynArr.Clear();
            Assert.AreEqual([], dynArr);
        }
    }

    [TestMethod]
    public void TestContains()
    {
        for (int i = 0; i < RepeatTime / 30 + 1; i++)
        {
            var mainArr = Generator.GetRandomIntArray(DefMaxSize, DefElementBound).ToList();
            DynArray<int> dynArr = mainArr.ToDynArray();
            var mainArrMax = mainArr.Max();
            for (int j = mainArr.Min(); j < mainArrMax; j++)
                Assert.AreEqual(mainArr.Contains(j), dynArr.Contains(j));
        }
    }
}

[TestClass]
public sealed class ResizeOps
{
    [TestMethod]
    public void TestGrowFactor()
    {
    }

    [TestMethod]
    public void TestReserve()
    {
    }

    [TestMethod]
    public void TestResize()
    {
    }

    [TestMethod]
    public void TestAdd()
    {
    }

    [TestMethod]
    public void TestInsert()
    {
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
        for (int i = 0; i < RepeatTime; i++)
        {

            DynArray<double> dynArr1 =
                Generator.GetRandomDoubleArray(DefMaxSize, DefElementBound).ToDynArray();
            DynArray<double> dynArr2;

            bool mustBeNotSame = (bool)Convert.ChangeType(Generator.Rand.Next(0, 2), typeof(bool));

            if (mustBeNotSame)
                dynArr2 = dynArr1.ToDynArray();
            else
                dynArr2 = dynArr1;

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
    }

    [TestMethod]
    public void TestToString()
    {
        for (int i = 0; i < RepeatTime; i++)
        {
            var dynArr = Generator.GetRandomIntArray(DefMaxSize, DefElementBound).ToDynArray();
            System.Text.StringBuilder sb = new System.Text.StringBuilder().Append("[");
            sb.Append(String.Join(", ", dynArr));
            sb.Append(']');
            Assert.AreEqual(sb.ToString(), dynArr.ToString());
        }
    }

    [TestMethod]
    public void TestEquals()
    {
    }

    [TestMethod]
    public void TestClone()
    {
    }

}


file static class ConstValues
{
    public static readonly Range DefMaxSize = ..600;

    public const int RepeatTime = 3000;
    public const int DefElementBound = 10000;
}
