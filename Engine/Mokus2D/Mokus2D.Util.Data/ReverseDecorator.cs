using System.Collections;
using System.Collections.Generic;

namespace Mokus2D.Util.Data;

public class ReverseDecorator<T> : IEnumerable<T>, IEnumerable
{
	private IList<T> source;

	public ReverseDecorator(IList<T> source)
	{
		this.source = source;
	}

	public IEnumerator<T> GetEnumerator()
	{
		return new ReverseEnumerator<T>(source);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
