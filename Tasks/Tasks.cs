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
        [10] = (Task1_10, "Видалити елементи між першим мінімальним і останнім максимальним"),
        [15] = (Task1_15, "Вставити 0 після кожного парного елемента"),
        [16] = (Task1_16, "Вставити 1 перед кожним парним елементом"),
    };

    public static readonly Dictionary<int, (Action<Context> task, string description)> Block2 = new()
    {
        [11] = (Task2_11, "Додати рядок після першого рядка з максимальним елементом"),
        [13] = (Task2_13, "Додати рядок перед першим рядком із мінімальним елементом"),
        [14] = (Task2_14, "Додати рядок після останнього рядка з мінімальним елементом"),
    };

    public static void Task1_10(Context context)
    {
        var arr = context.RequestArray(() => Generator.Rand.Next(-40, 40)).ToDynArray();
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

    public static void Task1_15(Context context)
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

    public static void Task1_16(Context context)
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


    public static void Task2_11(Context context)
    {
        var jaggedArray = context.RequestMatrix(int.Parse, () => Generator.Rand.Next(1, 101));

        int maxElement = int.MinValue;
        int maxRowIndex = 0;

        for (int i = 0; i < jaggedArray.Count; i++)
            foreach (var element in jaggedArray[i])
                if (element > maxElement)
                    (maxElement, maxRowIndex) = (element, i);

        jaggedArray.Insert(maxRowIndex + 1, [maxElement]);

        context.PrintLine("Result:");
        context.WriteLine(jaggedArray);
    }

    public static void Task2_13(Context context)
    {
        var arr = context.RequestMatrix(int.Parse, () => Generator.Rand.Next(-50, 50));

        var rowWithMinItem = arr.Enumerate()
                                .Map(x => (x.item.Min(), x.i))
                                .OrderBy(x => x.Item1)
                                .ThenBy(x => x.i)
                                .FirstOrDefault(x => true);

        arr.Insert(rowWithMinItem.i, [rowWithMinItem.Item1]);
        context.PrintLine($"Result:");
        context.WriteLine(arr);
    }

    public static void Task2_14(Context context)
    {
        var arr = context.RequestMatrix(int.Parse, () => Generator.Rand.Next(-50, 50));

        var rowWithMinItem = arr.Enumerate()
                                .Map(x => (x.item.Min(), x.i))
                                .OrderBy(x => x.Item1)
                                .ThenByDescending(x => x.i)
                                .FirstOrDefault(x => true);

        arr.Insert(rowWithMinItem.i + 1, [rowWithMinItem.Item1]);
        context.PrintLine($"Result:");
        context.WriteLine(arr);
    }
}
