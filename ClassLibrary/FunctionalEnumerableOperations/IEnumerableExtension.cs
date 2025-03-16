using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using ClassLibrary.Collections;

namespace ClassLibrary.FunctionalEnumerableOperations;

public static class IEnumerableExtension
{
    //Transforms
    public static IEnumerable<TResult> Map<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
        foreach (var item in source)
            yield return selector(item);
    }
    public static IEnumerable<TSource> Filter<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        foreach (var item in source)
            if (predicate(item))
                yield return item;
    }
    public static IEnumerable<TSource> Filter<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
    {
        var k = 0;
        foreach (var item in source)
            if (predicate(item, k++))
                yield return item;
    }
    public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
    {
        var enumerator = source.GetEnumerator();
        while (count-- > 0 && enumerator.MoveNext())
            yield return enumerator.Current;
    }
    public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        var enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
            if (predicate(enumerator.Current))
                yield return enumerator.Current;
            else break;
    }
    public static IEnumerable<TSource> Skip<TSource>(this IEnumerable<TSource> source, int count)
    {
        foreach (var item in source)
            if (count <= 0)
                yield return item;
            else count--;
    }
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
    public static IEnumerable<(TSource item, int i)> ToIndexedEnumerable<TSource>(this IEnumerable<TSource> source)
    {
        int k = 0;
        foreach (var item in source)
            yield return (item, k++);
    }
    //Bool aggregations
    public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        foreach (TSource element in source)
            if (!predicate(element))
                return false;
        return true;
    }
    public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        foreach (TSource element in source)
            if (predicate(element))
                return true;
        return false;
    }

    //Convertors
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
    public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
    {
        return [.. source];
    }
    public static DynArray<TSource> ToDynArray<TSource>(this IEnumerable<TSource> source)
    {
        return [.. source];
    }
    public static HashSet<TSource> ToSet<TSource>(this IEnumerable<TSource> source)
    {
        return [.. source];
    }

    //Folds
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
    public static TAccumulate Fold<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate initial, Func<TAccumulate, TSource, TAccumulate> func)
    {
        TAccumulate result = initial;
        foreach (var item in source)
            result = func(result, item);
        return result;
    }

    //Value finders
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
    public static TSource? FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        foreach (TSource value in source)
            if (predicate(value))
                return value;
        return default;
    }
    public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue)
    {
        foreach (TSource value in source)
            if (predicate(value))
                return value;
        return defaultValue;
    }

    //Sorters
    public static System.Linq.IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer = null)
    {
        return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer ?? Comparer<TKey>.Default, false);
    }
    public static System.Linq.IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this System.Linq.IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer = null)
    {
        return source.CreateOrderedEnumerable(keySelector, comparer ?? Comparer<TKey>.Default, false);
    }
    public static System.Linq.IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer = null)
    {
        return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer ?? Comparer<TKey>.Default, true);
    }
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
