using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

using Mokus2D.Util.MathUtils;

namespace Mokus2D.Util.Extensions
{
    public static class MokusCollectionExtensions
    {

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

        public static T RandomItem<T>(this List<T> source)
        {
            return source[Maths.Random(source.Count)];
        }

        public static void Resize<T>(this List<T> source, int newCount)
        {
            if (source == null)
            {
                throw new InvalidOperationException("List.Resize() - list is null.  Initialize it before use.");
            }
            _ = source.EnsureCapacity(newCount);
            while (source.Count < newCount)
            {
                source.Add(default);
            }
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

        public static T RemoveLast<T>(this IList<T> source)
        {
            T result = source[^1];
            if (source.Count > 0)
            {
                source.RemoveAt(source.Count - 1);
            }
            return result;
        }

        public static void AddItemsNoGarbage<T>(this List<T> list, IList<T> items, int start, int end)
        {
            _ = list.EnsureCapacity(list.Count + Math.Abs(end - start) + 1);
            int num = (end >= start) ? 1 : (-1);
            for (int i = start; i != end + num; i += num)
            {
                list.Add(items[i]);
            }
        }

    }
}
