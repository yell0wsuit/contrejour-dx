using System;

namespace Mokus2D.Util
{
    public static class Predicates<T>
    {
        public static readonly Predicate<T> True = o => true;

        public static readonly Predicate<T> False = o => false;
    }
}
