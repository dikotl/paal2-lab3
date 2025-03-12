using System.Collections;
using System.Numerics;

namespace FunctionalEnumerableOperations;

public static class IEnumerableExtension
{
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
    public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
    {
        int count = 0;
        foreach (var item in source) count++;
        var result = new TSource[count];
        int k = 0;
        foreach (var item in result) result[k++] = item;
        return result;
    }
    public static TSource Reduce<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
    {
        var enumerator = source.GetEnumerator();
        TSource result;
        if (enumerator.MoveNext()) result = enumerator.Current;
        else throw new InvalidOperationException();
        while (enumerator.MoveNext())
            result = func(result, enumerator.Current);
        return result;
    }
    public static TAccumulate Reduce<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate initial, Func<TAccumulate, TSource, TAccumulate> func)
    {
        TAccumulate result = initial;
        foreach (var item in source)
            result = func(result, item);
        return result;
    }
    //public static TResult Reduce<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate initial, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
    //{
    //    throw new NotSupportedException();
    //}
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
    public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue)
    {
        foreach (TSource value in source)
            if (predicate(value))
                return value;
        return defaultValue;
    }
    public static TSource? FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        foreach (TSource value in source)
            if (predicate(value))
                return value;
        return default;
    }


    public interface IOrderedEnumerable<out TElement> : IEnumerable<TElement>, IEnumerable
    {
        IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey>? comparer, bool descending);
    }
    /*public class OrderedEnumerable<TSource, TKey> : IOrderedEnumerable<TSource>
    {
        private readonly IEnumerable<TSource> source;
        private readonly Func<TSource, TKey> keySelector;
        private readonly IComparer<TKey> comparer;
        private readonly bool descending;
        private readonly IOrderedEnumerable<TSource>? parent;

        public OrderedEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer, bool descending)
        {
            this.source = source;
            this.keySelector = keySelector;
            this.comparer = comparer ?? Comparer<TKey>.Default;
            this.descending = descending;
        }

        private OrderedEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer, bool descending, IOrderedEnumerable<TSource> parent) : this(source, keySelector, comparer, descending)
        {
            this.parent = parent;
        }

        public IOrderedEnumerable<TSource> CreateOrderedEnumerable<TNextKey>(Func<TSource, TNextKey> nextKeySelector, IComparer<TNextKey>? nextComparer, bool nextDescending)
        {
            return new OrderedEnumerable<TSource, TNextKey>(source, nextKeySelector, nextComparer ?? Comparer<TNextKey>.Default, nextDescending, this);
        }

        public IEnumerator<TSource> GetEnumerator()
        {
            var array = source.ToArray();
            Array.Sort(array, new MultiLevelComparer(array, this));
            foreach (var item in array)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class MultiLevelComparer : IComparer<TSource>
        {
            private readonly OrderedEnumerable<TSource, TKey> root;
            private readonly Dictionary<TSource, TKey?> cache = new();

            public MultiLevelComparer(TSource[] elements, OrderedEnumerable<TSource, TKey> root)
            {
                this.root = root;
                foreach (var element in elements)
                    cache[element] = root.EvaluateKey(element);
            }

            public int Compare(TSource? x, TSource? y)
            {
                if (x == null) return y == null ? 0 : -1;
                if (y == null) return 1;
                return root.CompareKeys(x, y, cache);
            }
        }

        private TKey EvaluateKey(TSource item)
        {
            return keySelector(item);
        }

        private int CompareKeys(TSource x, TSource y, Dictionary<TSource, TKey?> cache)
        {
            int result = comparer.Compare(cache[x], cache[y]);
            if (result == 0 && parent != null)
                return parent.CreateOrderedEnumerable(keySelector, comparer, descending)
                             .GetEnumerator()
                             .MoveNext() ? 1 : -1;
            return descending ? -result : result;
        }
    }*/

    internal abstract class OrderedEnumerable<TElement> : IOrderedEnumerable<TElement>
    {
        internal IEnumerable<TElement> source;

        public IEnumerator<TElement> GetEnumerator()
        {
            Buffer<TElement> buffer = new Buffer<TElement>(source);
            if (buffer.count > 0)
            {
                EnumerableSorter<TElement> sorter = GetEnumerableSorter(null);
                int[] map = sorter.Sort(buffer.items, buffer.count);
                sorter = null;
                for (int i = 0; i < buffer.count; i++) yield return buffer.items[map[i]];
            }
        }

        internal abstract EnumerableSorter<TElement> GetEnumerableSorter(EnumerableSorter<TElement> next);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IOrderedEnumerable<TElement> IOrderedEnumerable<TElement>.CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending)
        {
            OrderedEnumerable<TElement, TKey> result = new OrderedEnumerable<TElement, TKey>(source, keySelector, comparer, descending);
            result.parent = this;
            return result;
        }
    }
    internal class OrderedEnumerable<TElement, TKey> : OrderedEnumerable<TElement>
    {
        internal OrderedEnumerable<TElement> parent;
        internal Func<TElement, TKey> keySelector;
        internal IComparer<TKey> comparer;
        internal bool descending;

        internal OrderedEnumerable(IEnumerable<TElement> source, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending)
        {
            this.source = source;
            this.parent = null;
            this.keySelector = keySelector;
            this.comparer = comparer != null ? comparer : Comparer<TKey>.Default;
            this.descending = descending;
        }

        internal override EnumerableSorter<TElement> GetEnumerableSorter(EnumerableSorter<TElement> next)
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
            Array.Sort(map);
            return map;
        }
    }
    internal class EnumerableSorter<TElement, TKey> : EnumerableSorter<TElement>
    {
        internal Func<TElement, TKey> keySelector;
        internal IComparer<TKey> comparer;
        internal bool descending;
        internal EnumerableSorter<TElement> next;
        internal TKey[] keys;

        internal EnumerableSorter(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending, EnumerableSorter<TElement> next)
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
    struct Buffer<TElement>
    {
        internal TElement[] items;
        internal int count;

        internal Buffer(IEnumerable<TElement> source)
        {
            TElement[]? items = null;
            int count = 0;
            ICollection<TElement> collection = (ICollection<TElement>)source;
            if (collection != null)
            {
                count = collection.Count;
                if (count > 0)
                {
                    items = new TElement[count];
                    collection.CopyTo(items, 0);
                }
            }
            else
            {
                foreach (TElement item in source)
                {
                    if (items == null)
                    {
                        items = new TElement[4];
                    }
                    else if (items.Length == count)
                    {
                        TElement[] newItems = new TElement[checked(count * 2)];
                        Array.Copy(items, 0, newItems, 0, count);
                        items = newItems;
                    }
                    items[count] = item;
                    count++;
                }
            }
            this.items = items;
            this.count = count;
        }

        internal TElement[] ToArray()
        {
            if (count == 0) return new TElement[0];
            if (items.Length == count) return items;
            TElement[] result = new TElement[count];
            Array.Copy(items, 0, result, 0, count);
            return result;
        }
    }

    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer = null)
    {
        return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer ?? Comparer<TKey>.Default, false);
    }

    public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer = null)
    {
        return source.CreateOrderedEnumerable(keySelector, comparer ?? Comparer<TKey>.Default, false);
    }

    public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer = null)
    {
        return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer ?? Comparer<TKey>.Default, true);
    }

    public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer = null)
    {
        return source.CreateOrderedEnumerable(keySelector, comparer ?? Comparer<TKey>.Default, true);
    }
}
