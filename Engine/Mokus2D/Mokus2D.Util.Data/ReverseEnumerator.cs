using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mokus2D.Util.Data;

public class ReverseEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable
{
	private IList<T> source;

	private int currentIndex;

	public T Current => source[currentIndex];

	object IEnumerator.Current => Current;

	public ReverseEnumerator(IList<T> source)
	{
		this.source = source;
		Reset();
	}

	public void Dispose()
	{
	}

	public bool MoveNext()
	{
		currentIndex--;
		return currentIndex >= 0;
	}

	public void Reset()
	{
		currentIndex = source.Count();
	}
}
