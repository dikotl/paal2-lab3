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
    }

    [TestMethod]
    public void TestInsert()
    {
    }

    [TestMethod]
    public void TestRemove()
    {
    }

    [TestMethod]
    public void TestAdd()
    {
    }

    [TestMethod]
    public void TestClear()
    {
    }

    [TestMethod]
    public void TestContains()
    {
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
