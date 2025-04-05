using System;
using System.Numerics;
using System.Collections;
using ClassLibraryCS.Collections;
using System.Collections.Generic;

namespace ClassLibraryCS.FunctionalEnumerableOperations;

public static class Enumerable
{
    // Transforms

    /// <summary>
    /// Projects each element of a sequence into a new form using the specified selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
    /// <typeparam name="TResult">The type of the elements in the resulting sequence.</typeparam>
    /// <param name="source">The sequence of elements to transform.</param>
    /// <param name="selector">A function to apply to each element.</param>
    /// <returns>An <see cref="IEnumerable{TResult}"/> with transformed elements.</returns>
    public static IEnumerable<TResult> Map<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
        foreach (var item in source)
            yield return selector(item);
    }

    /// <summary>
    /// Filters a sequence of elements based on a specified predicate.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The sequence to filter.</param>
    /// <param name="predicate">A function that determines whether an element should be included in the result.</param>
    /// <returns>An <see cref="IEnumerable{TSource}"/> containing only elements that satisfy the predicate.</returns>
    public static IEnumerable<TSource> Filter<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        foreach (var item in source)
            if (predicate(item))
                yield return item;
    }

    /// <summary>
    /// Filters a sequence of elements based on a specified predicate that also considers the element's index.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The sequence to filter.</param>
    /// <param name="predicate">A function that determines whether an element, based on its value and index, should be included in the result.</param>
    /// <returns>An <see cref="IEnumerable{TSource}"/> containing only elements that satisfy the predicate.</returns>
    public static IEnumerable<TSource> Filter<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
    {
        var k = 0;
        foreach (var item in source)
            if (predicate(item, k++))
                yield return item;
    }

    /// <summary>
    /// Returns a specified number of contiguous elements from the start of a sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The sequence to take elements from.</param>
    /// <param name="count">The number of elements to take from the start of the sequence.</param>
    /// <returns>An <see cref="IEnumerable{TSource}"/> containing the first <paramref name="count"/> elements of the sequence.</returns>
    public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
    {
        var enumerator = source.GetEnumerator();
        while (count-- > 0 && enumerator.MoveNext())
            yield return enumerator.Current;
    }

    /// <summary>
    /// Returns elements from the start of a sequence as long as the specified condition is true.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The sequence to take elements from.</param>
    /// <param name="predicate">A function to test each element for a condition. The elements are returned as long as the condition is true.</param>
    /// <returns>An <see cref="IEnumerable{TSource}"/> containing elements from the start of the sequence until the predicate fails.</returns>
    public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        var enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
            if (predicate(enumerator.Current))
                yield return enumerator.Current;
            else break;
    }

    /// <summary>
    /// Bypasses a specified number of elements in a sequence and returns the remaining elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The sequence to skip elements from.</param>
    /// <param name="count">The number of elements to skip.</param>
    /// <returns>An <see cref="IEnumerable{TSource}"/> containing the remaining elements after the specified number of elements has been skipped.</returns>
    public static IEnumerable<TSource> Skip<TSource>(this IEnumerable<TSource> source, int count)
    {
        foreach (var item in source)
            if (count <= 0)
                yield return item;
            else count--;
    }

    /// <summary>
    /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The sequence to skip elements from.</param>
    /// <param name="predicate">A function to test each element for a condition. The elements are skipped as long as the condition is true.</param>
    /// <returns>An <see cref="IEnumerable{TSource}"/> containing the remaining elements after the predicate condition becomes false.</returns>
    public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        var enumerator = source.GetEnumerator();
        bool dontBroke = true;
        while (enumerator.MoveNext())
            if (dontBroke && predicate(enumerator.Current))
                continue;
            else
            {
                yield return enumerator.Current;
                dontBroke = false;
            }
    }

    /// <summary>
    /// Enumerates a sequence and returns each element along with its index.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The sequence to enumerate.</param>
    /// <returns>An <see cref="IEnumerable{ValueTuple}"/> where each element is paired with its index in the sequence.</returns>
    public static IEnumerable<(TSource item, int i)> Enumerate<TSource>(this IEnumerable<TSource> source)
    {
        int k = 0;
        foreach (var item in source)
            yield return (item, k++);
    }

    /// <summary>
    /// Allows you to perform an operation on a value and return the result of that operation.
    /// </summary>
    /// <typeparam name="TSource">The type of the value to operate on.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by the operation.</typeparam>
    /// <param name="value">The value to apply the operation to.</param>
    /// <param name="func">A function that performs an operation on the value and returns a result.</param>
    /// <returns>The result of applying the function to the value.</returns>
    public static TResult Let<TSource, TResult>(this TSource value, Func<TSource, TResult> func)
    {
        return func(value);
    }


    // Bool aggregations

    /// <summary>
    /// Determines whether all elements of a sequence satisfy a specified condition.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The sequence to check.</param>
    /// <param name="predicate">A function that defines the condition to test each element against.</param>
    /// <returns><c>true</c> if every element in the sequence satisfies the condition; otherwise, <c>false</c>.</returns>
    public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        foreach (TSource element in source)
            if (!predicate(element))
                return false;
        return true;
    }

    /// <summary>
    /// Determines whether any element of a sequence satisfies a specified condition.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The sequence to check.</param>
    /// <param name="predicate">A function that defines the condition to test each element against.</param>
    /// <returns><c>true</c> if any element in the sequence satisfies the condition; otherwise, <c>false</c>.</returns>
    public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        foreach (TSource element in source)
            if (predicate(element))
                return true;
        return false;
    }


    // Convertors

    /// <summary>
    /// Creates an array from a sequence of elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The sequence to convert to an array.</param>
    /// <returns>An array containing the elements from the sequence.</returns>
    public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
    {
        TSource[]? items = null;
        int count = 0;
        ICollection<TSource>? collection = source as ICollection<TSource>;
        if (collection != null)
        {
            count = collection.Count;
            if (count > 0)
            {
                items = new TSource[count];
                collection.CopyTo(items, 0);
            }
        }
        else
        {
            foreach (TSource item in source)
            {
                if (items == null)
                {
                    items = new TSource[4];
                }
                else if (items.Length == count)
                {
                    TSource[] newItems = new TSource[checked(count * 2)];
                    Array.Copy(items, 0, newItems, 0, count);
                    items = newItems;
                }
                items[count] = item;
                count++;
            }
        }
        if (count == 0 || items == null) return [];
        if (items.Length == count) return items;
        TSource[] result = new TSource[count];
        Array.Copy(items, 0, result, 0, count);
        return result;
    }

    /// <summary>
    /// Creates a <see cref="List{T}"/> from a sequence of elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The sequence to convert to a list.</param>
    /// <returns>A <see cref="List{T}"/> containing the elements from the sequence.</returns>
    public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
    {
        return [.. source];
    }

    /// <summary>
    /// Creates a <see cref="DynArray{T}"/> from a sequence of elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The sequence to convert to a dynamic array.</param>
    /// <returns>A <see cref="DynArray{T}"/> containing the elements from the sequence.</returns>
    public static DynArray<TSource> ToDynArray<TSource>(this IEnumerable<TSource> source)
    {
        return [.. source];
    }

    /// <summary>
    /// Creates a <see cref="HashSet{T}"/> from a sequence of elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The sequence to convert to a hash set.</param>
    /// <returns>A <see cref="HashSet{T}"/> containing the elements from the sequence, with duplicates removed.</returns>
    public static HashSet<TSource> ToSet<TSource>(this IEnumerable<TSource> source)
    {
        return [.. source];
    }


    // Folds

    /// <summary>
    /// Performs a cumulative operation on a sequence, applying a function to each element and the accumulated result.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The sequence of elements to apply the operation on.</param>
    /// <param name="func">A function that combines the accumulated result with each element in the sequence.</param>
    /// <returns>The result of the fold operation over the sequence.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the source sequence is empty.</exception>
    public static TSource Fold<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
    {
        var enumerator = source.GetEnumerator();
        TSource result;
        if (enumerator.MoveNext()) result = enumerator.Current;
        else throw new InvalidOperationException();
        while (enumerator.MoveNext())
            result = func(result, enumerator.Current);
        return result;
    }

    /// <summary>
    /// Performs a cumulative operation on a sequence, applying a function to each element and an accumulator value.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
    /// <param name="source">The sequence of elements to apply the operation on.</param>
    /// <param name="initial">The initial value for the accumulator.</param>
    /// <param name="func">A function that combines the accumulator with each element in the sequence.</param>
    /// <returns>The final accumulated result after applying the function to each element.</returns>
    public static TAccumulate Fold<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate initial, Func<TAccumulate, TSource, TAccumulate> func)
    {
        TAccumulate result = initial;
        foreach (var item in source)
            result = func(result, item);
        return result;
    }


    // Value finders

    /// <summary>
    /// Returns the maximum element in a sequence based on the implemented comparison operators for the type.
    /// </summary>
    ///<typeparam name = "TSource" > The type of the elements in the sequence, which must implement<see cref="IComparisonOperators{TSelf,TOther,TResult}"/>.</typeparam>
    /// <param name="source">The sequence to find the maximum element from.</param>
    /// <returns>The maximum element in the sequence, or <c>default</c> if the sequence is empty.</returns>
    public static TSource? Max<TSource>(this IEnumerable<TSource> source) where TSource : IComparisonOperators<TSource, TSource, bool>
    {
        var enumerator = source.GetEnumerator();
        TSource? max;
        if (enumerator.MoveNext()) max = enumerator.Current;
        else return default;
        while (enumerator.MoveNext())
            if (max < enumerator.Current)
                max = enumerator.Current;
        return max;
    }

    /// <summary>
    /// Projects each element of a sequence to a value and returns the maximum value according to the provided selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <typeparam name="TResult">The type of the values returned by the selector function, which must implement <see cref="IComparisonOperators{TSelf,TOther,TResult}"/>.</typeparam>
    /// <param name="source">The sequence to find the maximum value from.</param>
    /// <param name="selector">A function to select a value from each element.</param>
    /// <returns>The maximum value selected from the sequence, or <c>default</c> if the sequence is empty.</returns>
    public static TResult? Max<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
            where TResult : IComparisonOperators<TResult, TResult, bool>
    {
        var enumerator = source.GetEnumerator();
        TResult? max;
        if (enumerator.MoveNext()) max = selector(enumerator.Current);
        else return default;
        while (enumerator.MoveNext())
            if (max < selector(enumerator.Current))
                max = selector(enumerator.Current);
        return max;
    }

    /// <summary>
    /// Returns the minimum element in a sequence based on the implemented comparison operators for the type.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence, which must implement <see cref="IComparisonOperators{TSelf,TOther,TResult}"/>.</typeparam>
    /// <param name="source">The sequence to find the minimum element from.</param>
    /// <returns>The minimum element in the sequence, or <c>default</c> if the sequence is empty.</returns>
    public static TSource? Min<TSource>(this IEnumerable<TSource> source) where TSource : IComparisonOperators<TSource, TSource, bool>
    {
        var enumerator = source.GetEnumerator();
        TSource? min;
        if (enumerator.MoveNext()) min = enumerator.Current;
        else return default;
        while (enumerator.MoveNext())
            if (min > enumerator.Current)
                min = enumerator.Current;
        return min;
    }

    /// <summary>
    /// Projects each element of a sequence to a value and returns the minimum value according to the provided selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <typeparam name="TResult">The type of the values returned by the selector function, which must implement <see cref="IComparisonOperators{TSelf,TOther,TResult}"/>.</typeparam>
    /// <param name="source">The sequence to find the minimum value from.</param>
    /// <param name="selector">A function to select a value from each element.</param>
    /// <returns>The minimum value selected from the sequence, or <c>default</c> if the sequence is empty.</returns>
    public static TResult? Min<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
            where TResult : IComparisonOperators<TResult, TResult, bool>
    {
        var enumerator = source.GetEnumerator();
        TResult? min;
        if (enumerator.MoveNext()) min = selector(enumerator.Current);
        else return default;
        while (enumerator.MoveNext())
            if (min > selector(enumerator.Current))
                min = selector(enumerator.Current);
        return min;
    }

    /// <summary>
    /// Returns the first element in a sequence, or <c>default</c> if no element is found.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The sequence to find the first element from.</param>
    /// <returns>The first element in the sequence, or <c>default</c> if the sequence is empty.</returns>
    public static TSource? FirstOrDefault<TSource>(this IEnumerable<TSource> source)
    {
        var enumerator = source.GetEnumerator();
        if (enumerator.MoveNext())
            return enumerator.Current;
        else
            return default;
    }

    /// <summary>
    /// Returns the first element in a sequence that satisfies a specified condition, or <c>default</c> if no such element is found.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The sequence to search for the first element.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>The first element in the sequence that satisfies the condition, or <c>default</c> if no such element is found.</returns>
    public static TSource? FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        foreach (TSource value in source)
            if (predicate(value))
                return value;
        return default;
    }

    /// <summary>
    /// Returns the first element in a sequence that satisfies a specified condition, or a specified default value if no such element is found.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The sequence to search for the first element.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="defaultValue">The value to return if no element satisfies the condition.</param>
    /// <returns>The first element in the sequence that satisfies the condition, or the specified <paramref name="defaultValue"/> if no such element is found.</returns>
    public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue)
    {
        foreach (TSource value in source)
            if (predicate(value))
                return value;
        return defaultValue;
    }


    // Sorters

    /// <summary>
    /// Sorts the elements of a sequence in ascending order according to a key selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key used to compare elements.</typeparam>
    /// <param name="source">The sequence to sort.</param>
    /// <param name="keySelector">A function to extract the key to compare elements by.</param>
    /// <param name="comparer">An optional <see cref="IComparer{TKey}"/> to compare keys, or <c>null</c> to use the default comparer.</param>
    /// <returns>An ordered sequence of elements in ascending order based on the selected key.</returns>
    public static System.Linq.IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer = null)
    {
        return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer ?? Comparer<TKey>.Default, false);
    }

    /// <summary>
    /// Performs a subsequent ordering of elements in a sequence in ascending order according to a key selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key used to compare elements.</typeparam>
    /// <param name="source">The previously ordered sequence to further sort.</param>
    /// <param name="keySelector">A function to extract the key to compare elements by.</param>
    /// <param name="comparer">An optional <see cref="IComparer{TKey}"/> to compare keys, or <c>null</c> to use the default comparer.</param>
    /// <returns>An ordered sequence of elements in ascending order based on the selected key.</returns>
    public static System.Linq.IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this System.Linq.IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer = null)
    {
        return source.CreateOrderedEnumerable(keySelector, comparer ?? Comparer<TKey>.Default, false);
    }

    /// <summary>
    /// Sorts the elements of a sequence in descending order according to a key selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key used to compare elements.</typeparam>
    /// <param name="source">The sequence to sort.</param>
    /// <param name="keySelector">A function to extract the key to compare elements by.</param>
    /// <param name="comparer">An optional <see cref="IComparer{TKey}"/> to compare keys, or <c>null</c> to use the default comparer.</param>
    /// <returns>An ordered sequence of elements in descending order based on the selected key.</returns>
    public static System.Linq.IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer = null)
    {
        return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer ?? Comparer<TKey>.Default, true);
    }

    /// <summary>
    /// Performs a subsequent ordering of elements in a sequence in descending order according to a key selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key used to compare elements.</typeparam>
    /// <param name="source">The previously ordered sequence to further sort.</param>
    /// <param name="keySelector">A function to extract the key to compare elements by.</param>
    /// <param name="comparer">An optional <see cref="IComparer{TKey}"/> to compare keys, or <c>null</c> to use the default comparer.</param>
    /// <returns>An ordered sequence of elements in descending order based on the selected key.</returns>
    public static System.Linq.IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this System.Linq.IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer = null)
    {
        return source.CreateOrderedEnumerable(keySelector, comparer ?? Comparer<TKey>.Default, true);
    }

}

internal abstract class OrderedEnumerable<TElement> : System.Linq.IOrderedEnumerable<TElement>
{
    internal IEnumerable<TElement> source;

    public IEnumerator<TElement> GetEnumerator()
    {
        TElement[] buffer = source.ToArray();
        if (buffer.Length > 0)
        {
            EnumerableSorter<TElement>? sorter = GetEnumerableSorter(null);
            int[] map = sorter.Sort(buffer, buffer.Length);
            sorter = null;
            for (int i = 0; i < buffer.Length; i++) yield return buffer[map[i]];
        }
    }

    internal abstract EnumerableSorter<TElement> GetEnumerableSorter(EnumerableSorter<TElement>? next);
    internal OrderedEnumerable(IEnumerable<TElement> source)
    {
        this.source = source;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    System.Linq.IOrderedEnumerable<TElement> System.Linq.IOrderedEnumerable<TElement>.CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey>? comparer, bool descending)
    {
        OrderedEnumerable<TElement, TKey> result = new OrderedEnumerable<TElement, TKey>(source, keySelector, comparer, descending);
        result.parent = this;
        return result;
    }
}
internal class OrderedEnumerable<TElement, TKey> : OrderedEnumerable<TElement>
{
    internal OrderedEnumerable<TElement>? parent;
    internal Func<TElement, TKey> keySelector;
    internal IComparer<TKey> comparer;
    internal bool descending;

    internal OrderedEnumerable(IEnumerable<TElement> source, Func<TElement, TKey> keySelector, IComparer<TKey>? comparer, bool descending) : base(source)
    {
        this.parent = null;
        this.keySelector = keySelector;
        this.comparer = comparer != null ? comparer : Comparer<TKey>.Default;
        this.descending = descending;
    }

    internal override EnumerableSorter<TElement> GetEnumerableSorter(EnumerableSorter<TElement>? next)
    {
        EnumerableSorter<TElement> sorter = new EnumerableSorter<TElement, TKey>(keySelector, comparer, descending, next);
        if (parent != null) sorter = parent.GetEnumerableSorter(sorter);
        return sorter;
    }
}
internal abstract class EnumerableSorter<TElement>
{
    internal abstract void ComputeKeys(TElement[] elements, int count);

    internal abstract int CompareKeys(int index1, int index2);

    internal int[] Sort(TElement[] elements, int count)
    {
        ComputeKeys(elements, count);
        int[] map = new int[count];
        for (int i = 0; i < count; i++) map[i] = i;
        QuickSort(map, 0, count - 1);
        return map;
    }

    void QuickSort(int[] map, int left, int right)
    {
        do
        {
            int i = left;
            int j = right;
            int x = map[i + ((j - i) >> 1)];
            do
            {
                while (i < map.Length && CompareKeys(x, map[i]) > 0) i++;
                while (j >= 0 && CompareKeys(x, map[j]) < 0) j--;
                if (i > j) break;
                if (i < j)
                {
                    int temp = map[i];
                    map[i] = map[j];
                    map[j] = temp;
                }
                i++;
                j--;
            } while (i <= j);
            if (j - left <= right - i)
            {
                if (left < j) QuickSort(map, left, j);
                left = i;
            }
            else
            {
                if (i < right) QuickSort(map, i, right);
                right = j;
            }
        } while (left < right);
    }
}
internal class EnumerableSorter<TElement, TKey> : EnumerableSorter<TElement>
{
    internal Func<TElement, TKey> keySelector;
    internal IComparer<TKey> comparer;
    internal bool descending;
    internal EnumerableSorter<TElement>? next;
    internal TKey[] keys = [];

    internal EnumerableSorter(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending, EnumerableSorter<TElement>? next)
    {
        this.keySelector = keySelector;
        this.comparer = comparer;
        this.descending = descending;
        this.next = next;
    }

    internal override void ComputeKeys(TElement[] elements, int count)
    {
        keys = new TKey[count];
        for (int i = 0; i < count; i++) keys[i] = keySelector(elements[i]);
        if (next != null) next.ComputeKeys(elements, count);
    }

    internal override int CompareKeys(int index1, int index2)
    {
        int c = comparer.Compare(keys[index1], keys[index2]);
        if (c == 0)
        {
            if (next == null) return index1 - index2;
            return next.CompareKeys(index1, index2);
        }
        return descending ? -c : c;
    }
}