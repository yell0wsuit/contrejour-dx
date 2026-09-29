using System;

namespace Mokus2D.Util.Extensions
{
    public static class FunctionExtensions
    {
        public static bool NullOrTrue<T>(this Predicate<T> predicate, T obj)
        {
            return predicate?.Invoke(obj) ?? true;
        }
    }
}
