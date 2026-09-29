using System.Globalization;

using Default.Namespace;

using Mokus2D.Util.Data;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Data;

namespace System.Collections.Generic;

public static class MokusCollectionExtensions
{
    public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
    {
        _ = source.TryGetValue(key, out TValue value);
        return value;
    }

    public static int IndexOf<T>(this T[] array, T element)
    {
        return Array.IndexOf(array, element);
    }

    public static bool HasKey(this IDictionary<string, string> source, string key)
    {
        return source?.ContainsKey(key) ?? false;
    }

    public static float GetFloat(this IDictionary<string, string> source, string key, float defaultValue)
    {
        return source == null || !source.ContainsKey(key) ? defaultValue : source.GetFloat(key);
    }

    public static string GetString(this IDictionary<string, string> source, string key, string defaultValue = null)
    {
        return source != null && source.TryGetValue(key, out string value) ? value : defaultValue;
    }

    public static float GetFloat(this IDictionary<string, string> source, string key)
    {
        return System.Convert.ToSingle(source[key], CultureInfo.InvariantCulture.NumberFormat);
    }

    public static bool GetBool(this IDictionary<string, string> source, string key, bool defaultValue = false)
    {
        return source.TryGetValue(key, out string value) ? System.Convert.ToBoolean(value, CultureInfo.InvariantCulture) : defaultValue;
    }

    public static int GetInt(this IDictionary<string, string> source, string key, int defaultValue)
    {
        return source == null || !source.ContainsKey(key) ? defaultValue : source.GetInt(key);
    }

    public static int GetInt(this IDictionary<string, string> source, string key)
    {
        return System.Convert.ToInt32(source[key], CultureInfo.InvariantCulture);
    }

    public static double GetDouble(this IDictionary<string, string> dictionary, string key)
    {
        return System.Convert.ToDouble(dictionary[key], CultureInfo.InvariantCulture);
    }

    public static void SortOn<T>(this List<T> list, Func<T, float> field)
    {
        list.Sort((i, j) => Comparisons.FloatComparizon(field(i), field(j)));
    }

    public static bool NullOrEmpty<T>(this IList<T> list)
    {
        return list?.Empty() ?? true;
    }

    public static bool Empty<T>(this List<T> list)
    {
        return list.Count == 0;
    }

    public static bool Empty<T>(this ICollection<T> list)
    {
        return list.Count == 0;
    }

    public static bool Empty(this ICollection list)
    {
        return list.Count == 0;
    }

    public static T RandomItem<T>(this List<T> source)
    {
        return source[Maths.Random(source.Count)];
    }

    public static T Min<T>(this IList<T> source, IComparer<T> comparer) where T : class
    {
        if (source.Count == 0)
        {
            return null;
        }
        T val = source.First();
        for (int i = 1; i < source.Count; i++)
        {
            T val2 = source[i];
            if (comparer.Compare(val, val2) < 0)
            {
                val = val2;
            }
        }
        return val;
    }

    public static T Last<T>(this IList<T> source, Predicate<T> predicate)
    {
        return FirstFromSide(source, predicate, source.Count - 1, -1);
    }

    public static T First<T>(this IList<T> source, Predicate<T> predicate)
    {
        return FirstFromSide(source, predicate, 0, 1);
    }

    private static T FirstFromSide<T>(IList<T> source, Predicate<T> predicate, int start, int diff)
    {
        for (int i = start; Maths.Between(i, 0f, source.Count - 1); i += diff)
        {
            T val = source[i];
            if (predicate(val))
            {
                return val;
            }
        }
        return default;
    }

    public static T Last<T>(this IList<T> source)
    {
        return source[source.Count - 1];
    }

    public static void SetLast<T>(this IList<T> source, T value)
    {
        source[source.Count - 1] = value;
    }

    public static T FindAny<T>(this IList<T> source, Predicate<T> predicate)
    {
        foreach (T item in source)
        {
            if (predicate(item))
            {
                return item;
            }
        }
        return default;
    }

    public static T First<T>(this IList<T> source)
    {
        return source[0];
    }

    public static bool Contains<T>(this T[] source, T item)
    {
        return Array.IndexOf(source, item) >= 0;
    }

    public static bool Contains<T>(this IList<T> source, Func<T, bool> filter)
    {
        foreach (T item in source)
        {
            if (filter(item))
            {
                return true;
            }
        }
        return false;
    }

    public static void EnsureCapacity<T>(this List<T> source, int capacity)
    {
        source.Capacity = Math.Max(capacity, source.Capacity);
    }

    public static void Resize<T>(this List<T> source, int newCount, Func<T> allocateFunc)
    {
        if (source == null)
        {
            throw new InvalidOperationException("List.Resize() - list is null.  Initialize it before use.");
        }
        EnsureCapacity(source, newCount);
        while (source.Count < newCount)
        {
            source.Add(allocateFunc());
        }
    }

    public static void Resize<T>(this List<T> source, int newCount)
    {
        if (source == null)
        {
            throw new InvalidOperationException("List.Resize() - list is null.  Initialize it before use.");
        }
        EnsureCapacity(source, newCount);
        while (source.Count < newCount)
        {
            source.Add(default);
        }
    }

    public static bool Exists<T>(this IList<T> source, T element)
    {
        return source.IndexOf(element) != -1;
    }

    public static bool NotExists<T>(this IList<T> source, T element)
    {
        return source.IndexOf(element) == -1;
    }

    public static void RemoveList<T>(this IList<T> source, IEnumerable toRemove)
    {
        foreach (object item in toRemove)
        {
            _ = source.Remove((T)item);
        }
    }

    public static void RemoveListNoGarbage<T>(this IList<T> source, IList<T> toRemove)
    {
        for (int i = 0; i < toRemove.Count; i++)
        {
            _ = source.Remove(toRemove[i]);
        }
    }

    public static ReverseListCollection<T> ReverseForEach<T>(this IList<T> list)
    {
        return new ReverseListCollection<T>(list);
    }

    public static ReverseListEnumerator<T> GetReverseEnumerator<T>(this IList<T> list)
    {
        return new ReverseListEnumerator<T>(list);
    }

    public static bool SafeRemove<T>(this IList<T> source, T value)
    {
        int num = source.IndexOf(value);
        if (num >= 0)
        {
            source.RemoveAt(num);
            return true;
        }
        return false;
    }

    public static T RemoveLast<T>(this IList<T> source)
    {
        T result = source.Last();
        if (source.Count > 0)
        {
            source.RemoveAt(source.Count - 1);
        }
        return result;
    }

    public static List<TTarget> Convert<TSource, TTarget>(this List<TSource> source, Func<TSource, TTarget> converter)
    {
        List<TTarget> result = new(source.Count);
        source.Convert(result, converter);
        return result;
    }

    public static void Convert<TSource, TTarget>(this List<TSource> source, List<TTarget> result, Func<TSource, TTarget> converter)
    {
        foreach (TSource item in source)
        {
            result.Add(converter(item));
        }
    }

    public static List<TTarget> Convert<TSource, TTarget>(this IList<TSource> source, Func<TSource, TTarget> converter)
    {
        List<TTarget> result = new(source.Count);
        source.Convert(result, converter);
        return result;
    }

    public static void Convert<TSource, TTarget>(this IList<TSource> source, List<TTarget> result, Func<TSource, TTarget> converter)
    {
        for (int i = 0; i < source.Count; i++)
        {
            TSource arg = source[i];
            result.Add(converter(arg));
        }
    }

    public static IEnumerable<TResult> Convert<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> converter)
    {
        foreach (TSource element in source)
        {
            yield return converter(element);
        }
    }

    public static void Each<T>(this IEnumerable<T> collection, Action<T, int> action)
    {
        int num = 0;
        foreach (T item in collection)
        {
            action(item, num++);
        }
    }

    public static void Each<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (T item in collection)
        {
            action(item);
        }
    }

    public static IEnumerable<int> Range(this int max)
    {
        for (int i = 0; i < max; i++)
        {
            yield return i;
        }
    }

    public static void Times(this int i, Action<int> action)
    {
        i.Range().Each(action);
    }

    public static void Add<T>(this List<T> list, T item, int count)
    {
        EnsureCapacity(list, list.Count + count);
        for (int i = 0; i < count; i++)
        {
            list.Add(item);
        }
    }

    public static void AddItemsNoGarbage<T>(this List<T> list, IList<T> items, int start, int end)
    {
        EnsureCapacity(list, list.Count + Math.Abs(end - start) + 1);
        int num = (end >= start) ? 1 : (-1);
        for (int i = start; i != end + num; i += num)
        {
            list.Add(items[i]);
        }
    }

    public static void AddCastedItemsNoGarbage<T1, T2>(this List<T1> list, IList<T2> items, int start, int end)
    {
        EnsureCapacity(list, list.Count + Math.Abs(end - start) + 1);
        int num = (end >= start) ? 1 : (-1);
        for (int i = start; i != end + num; i += num)
        {
            list.Add((T1)(object)items[i]);
        }
    }

    public static void AddCastedItemsNoGarbage<T1, T2>(this List<T1> list, IList<T2> items)
    {
        if (items.Count != 0)
        {
            list.AddCastedItemsNoGarbage(items, 0, items.Count - 1);
        }
    }

    public static void AddItemsNoGarbage<T>(this List<T> list, IList<T> items)
    {
        if (items.Count != 0)
        {
            list.AddItemsNoGarbage(items, 0, items.Count - 1);
        }
    }

    public static int Count<TSource>(this List<TSource> list, Predicate<TSource> predicate)
    {
        int num = 0;
        foreach (TSource item in list)
        {
            if (predicate(item))
            {
                num++;
            }
        }
        return num;
    }

    public static void Fill<T>(this List<T> list, T item, int count)
    {
        EnsureCapacity(list, list.Count + count);
        for (int i = 0; i < count; i++)
        {
            list.Add(item);
        }
    }

    public static void Times(this int times, Action action)
    {
        for (int i = 0; i < times; i++)
        {
            action();
        }
    }

    public static List<object> Filter(IList objects, Predicate<object> match)
    {
        List<object> list = [];
        foreach (object @object in objects)
        {
            if (match(@object))
            {
                list.Add(@object);
            }
        }
        return list;
    }

}
