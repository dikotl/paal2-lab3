using System;
using System.Collections.Generic;
using ClassLibrary.Collections;
using ClassLibrary.FunctionalEnumerableOperations;
using ClassLibraryVB.IO;

namespace App;

public static class Program
{
    public static readonly Dictionary<int, (Action<Context> task, string description)> Block1Tasks = new()
    {
        [10] = (Task10, "Знищити всі елементи між першим із мінімальних за значенням і останнім із максимальних за значенням; самі перший з мінімальних та останній з максимальних лишити (не знищувати); врахувати, що невідомо, який з них записано в масиві раніше"),
        [15] = (Task15, "Вставити після кожного парного елемента елемент із значенням 0"),
        [16] = (Task16, "Вставити перед кожним парним елементом елемент із значенням 1"),
    };

    public static readonly Dictionary<int, (Action<Context> task, string description)> Block2Tasks = new()
    {
        [11] = (Task11, "Додати рядок після рядка, що містить максимальний елемент (якщо у різних місцях є кілька елементів з однаковим максимальним значенням, то брати перший з них)"),
        [13] = (Task13, "Додати рядок перед рядком, що містить мінімальний елемент (якщо у різних місцях є кілька елементів з однаковим мінімальним значенням, то брати перший з них)"),
        [14] = (Task14, "Додати рядок після рядка, що містить мінімальний елемент (якщо у різних місцях є кілька елементів з однаковим мінімальним значенням, то брати останній з них)"),
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

        context.PrintLine("Result:");
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
        context.PrintLine($"Result:");
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
        context.PrintLine($"Result:");
        foreach (var item in arr)
            context.WriteLine(item);
    }
}
