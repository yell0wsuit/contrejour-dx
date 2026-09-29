using System.Collections.Generic;

namespace Mokus2D.Util.Extensions
{
    public static class HashSetExtensions
    {
        public static void AddNewItems<T>(this HashSet<T> target, List<T> items)
        {
            foreach (T item in items)
            {
                if (!target.Contains(item))
                {
                    _ = target.Add(item);
                }
            }
        }

        public static void AddAll<T>(this HashSet<T> target, List<T> items)
        {
            foreach (T item in items)
            {
                _ = target.Add(item);
            }
        }

        public static void AddNewItems<T>(this HashSet<T> target, HashSet<T> items)
        {
            foreach (T item in items)
            {
                if (!target.Contains(item))
                {
                    _ = target.Add(item);
                }
            }
        }
    }
}
