using System.Collections.Generic;

using Mokus2D.Effects.Tweening;

namespace Mokus2D.Util.Data
{
    public struct Pair<T>(T first, T second)
    {
        public static readonly StructGetSet<Pair<T>, T> FirstValue = new(delegate (ref Pair<T> p)
        {
            return p.First;
        }, delegate (ref Pair<T> p, T v)
        {
            p.First = v;
        });

        public static readonly StructGetSet<Pair<T>, T> SecondValue = new(delegate (ref Pair<T> p)
        {
            return p.Second;
        }, delegate (ref Pair<T> p, T v)
        {
            p.Second = v;
        });

        public T First = first;

        public T Second = second;

        public override readonly int GetHashCode()
        {
            return (EqualityComparer<T>.Default.GetHashCode(First) * 397) ^ EqualityComparer<T>.Default.GetHashCode(Second);
        }
    }
}
