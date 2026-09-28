using System;
using System.Collections.Generic;

namespace Mokus2D.Visual.Data;

public class ComparisonComparer<T> : IComparer<T>
{
	private readonly Comparison<T> _comparison;

	public ComparisonComparer(Comparison<T> comparison)
	{
		_comparison = comparison;
	}

	public int Compare(T x, T y)
	{
		return _comparison(x, y);
	}
}
