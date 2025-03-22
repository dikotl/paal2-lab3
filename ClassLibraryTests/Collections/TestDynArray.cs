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
    }

    [TestMethod]
    public void TestGetCount()
    {
    }

    [TestMethod]
    public void TestGetCapacity()
    {
    }

    [TestMethod]
    public void TestGetEnumerator()
    {
    }

    [TestMethod]
    public void TestCtors()
    {
    }

    [TestMethod]
    public void TestSlice()
    {
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
            DynArray<double> dynArr = mainArr.ToDynArray(); // ToDynArray must pass the test in Feo tests
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
            DynArray<double> dynArr = mainArr.ToDynArray(); // ToDynArray must pass the test in Feo tests
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
            DynArray<double> dynArr = mainArr.ToDynArray(); // ToDynArray must pass the test in Feo tests
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
            // ToDynArray must pass the test in Feo tests
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
            DynArray<int> dynArr = mainArr.ToDynArray(); // ToDynArray must pass the test in Feo tests
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
