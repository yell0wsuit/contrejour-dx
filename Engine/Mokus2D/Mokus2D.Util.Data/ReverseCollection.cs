using System.Collections;
using System.Collections.Generic;

namespace Mokus2D.Util.Data;

public class ReverseCollection<T> : IEnumerable<T>, IEnumerable
{
    private readonly IList<T> source;

    public ReverseCollection(IList<T> source)
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
