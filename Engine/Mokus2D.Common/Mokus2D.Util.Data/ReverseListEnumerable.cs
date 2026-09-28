using System.Collections;
using System.Collections.Generic;

namespace Mokus2D.Util.Data;

public struct ReverseListEnumerable<T>(IList<T> list) : IEnumerable<T>, IEnumerable
{
    private readonly IList<T> _list = list;

    public IEnumerator<T> GetEnumerator()
    {
        return new ReverseListEnumerator<T>(_list);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
