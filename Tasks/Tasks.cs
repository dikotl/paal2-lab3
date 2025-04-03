using System;
using System.Collections.Generic;
using ClassLibraryCS.Collections;
using ClassLibraryVB.IO;
using ClassLibraryCS.FunctionalEnumerableOperations;

namespace Tasks;

public static class Tasks
{
    public static readonly Dictionary<int, (Action<Context> task, string description)> Block1 = new()
    {
        [10] = (Task10, "Видалити елементи між першим мінімальним і останнім максимальним"),
        [15] = (Task15, "Вставити 0 після кожного парного елемента"),
        [16] = (Task16, "Вставити 1 перед кожним парним елементом"),
    };

    public static readonly Dictionary<int, (Action<Context> task, string description)> Block2 = new()
    {
        [11] = (Task11, "Додати рядок після першого рядка з максимальним елементом"),
        [13] = (Task13, "Додати рядок перед першим рядком із мінімальним елементом"),
        [14] = (Task14, "Додати рядок після останнього рядка з мінімальним елементом"),
    };

    public static void Task10(Context context)
    {
        var arr = context.RequestArray(() => Generator.Rand.Next(-99, 99)).ToDynArray();
        DelElements();
        context.WriteLine(arr);

        void DelElements()
        {
            if (arr.Count < 2) return;
            int FirstMinIndex = 0;
            int LastMaxIndex = 0;

            for (int i = 0; i < arr.Count; i++)
            {
                if (arr[i] < arr[FirstMinIndex])
                    FirstMinIndex = i;
                if (arr[i] >= arr[LastMaxIndex])
                    LastMaxIndex = i;
            }

            context.PrintLine($"max index: {LastMaxIndex}");
            context.PrintLine($"min index: {FirstMinIndex}");

            if (FirstMinIndex > LastMaxIndex)
                (FirstMinIndex, LastMaxIndex) = (LastMaxIndex, FirstMinIndex);

            arr = arr.Take(FirstMinIndex + 1).ToDynArray() + arr.Skip(LastMaxIndex).ToDynArray();
        }

    }

    public static void Task15(Context context)
    {
        DynArray<int> input = context.RequestArray(() => Generator.Rand.Next(-999, 999));
        DynArray<int> result = input
            .Enumerate()
            .Filter(e => e.item % 2 == 0)
            .Map(e => e.i + 1)
            .Fold(input[..], WithInsert)
            .ToDynArray();

        context.WriteLine(result);


        DynArray<int> WithInsert(DynArray<int> accumulator, int index)
        {
            int delta = accumulator.Count - input.Count;
            accumulator.Insert(index + delta, 0);
            return accumulator;
        }
    }

    public static void Task16(Context context)
    {
        DynArray<int> input = context.RequestArray(() => Generator.Rand.Next(-999, 999));
        DynArray<int> result = input
            .Enumerate()
            .Filter(e => e.item % 2 == 0)
            .Map(e => e.i)
            .Fold(input[..], WithInsert)
            .ToDynArray();

        context.WriteLine(result);


        DynArray<int> WithInsert(DynArray<int> accumulator, int index)
        {
            int delta = accumulator.Count - input.Count;
            accumulator.Insert(index + delta, 1);
            return accumulator;
        }
    }


    public static void Task11(Context context)
    {
        var jaggedArray = context.RequestMatrix(int.Parse, () => Generator.Rand.Next(1, 101));

        int maxElement = int.MinValue;
        int maxRowIndex = 0;

        for (int i = 0; i < jaggedArray.Count; i++)
            foreach (var element in jaggedArray[i])
                if (element > maxElement)
                    (maxElement, maxRowIndex) = (element, i);

        jaggedArray.Insert(maxRowIndex + 1, [99, 100]);

        context.PrintLine("Result:", ConsoleColor.DarkCyan);
        foreach (var row in jaggedArray)
            context.WriteLine(row);
    }

    public static void Task13(Context context)
    {
        var arr = context.RequestMatrix(int.Parse, () => Generator.Rand.Next(-20, 20));

        var rowWithMinItem = arr.Enumerate()
                                .Map(x => (x.item.Min(), x.i))
                                .OrderBy(x => x.Item1)
                                .ThenBy(x => x.i)
                                .FirstOrDefault(x => true);

        arr.Insert(rowWithMinItem.i, [rowWithMinItem.Item1]);
        context.PrintLine($"Result:", ConsoleColor.DarkCyan);
        foreach (var item in arr)
            context.WriteLine(item);
    }

    public static void Task14(Context context)
    {
        var arr = context.RequestMatrix(int.Parse, () => Generator.Rand.Next(-20, 20));

        var rowWithMinItem = arr.Enumerate()
                                .Map(x => (x.item.Min(), x.i))
                                .OrderBy(x => x.Item1)
                                .ThenByDescending(x => x.i)
                                .FirstOrDefault(x => true);

        arr.Insert(rowWithMinItem.i + 1, [rowWithMinItem.Item1]);
        context.PrintLine($"Result:", ConsoleColor.DarkCyan);
        foreach (var item in arr)
            context.WriteLine(item);
    }
}
