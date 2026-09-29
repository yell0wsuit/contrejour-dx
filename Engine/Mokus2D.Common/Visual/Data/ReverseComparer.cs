using System;
using System.Collections.Generic;

namespace Mokus2D.Visual.Data;

public class ReverseComparer<T>(IComparer<T> comparer) : IComparer<T>
{
    public static readonly ReverseComparer<T> Default = new(Comparer<T>.Default);

    private readonly IComparer<T> _comparer = comparer;

    public ReverseComparer(Comparison<T> comparison)
        : this(new ComparisonComparer<T>(comparison))
    {
    }

    public int Compare(T x, T y)
    {
        return _comparer.Compare(x, y) * -1;
    }
}
