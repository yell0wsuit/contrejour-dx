using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

using Mokus2D.Util.MathUtils;

namespace Mokus2D.Util.Extensions
{
    public static class MokusCollectionExtensions
    {
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            _ = source.TryGetValue(key, out TValue value);
            return value;
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
            return Convert.ToSingle(source[key], CultureInfo.InvariantCulture.NumberFormat);
        }

        public static bool GetBool(this IDictionary<string, string> source, string key, bool defaultValue = false)
        {
            return source.TryGetValue(key, out string value) ? Convert.ToBoolean(value, CultureInfo.InvariantCulture) : defaultValue;
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

        public static T First<T>(this IList<T> source)
        {
            return source[0];
        }

        public static void EnsureCapacity<T>(this List<T> source, int capacity)
        {
            source.Capacity = Math.Max(capacity, source.Capacity);
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

        public static void Each<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection)
            {
                action(item);
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

        public static void AddItemsNoGarbage<T>(this List<T> list, IList<T> items)
        {
            if (items.Count != 0)
            {
                list.AddItemsNoGarbage(items, 0, items.Count - 1);
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
}
