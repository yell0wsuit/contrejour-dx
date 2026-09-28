using System;
using System.Collections;
using System.Collections.Generic;

namespace Mokus2D.Util.Data;

public struct ReverseListEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable
{
    private readonly IList<T> _list;

    private int _currentIndex;

    public T Current => _list[_currentIndex];

    object IEnumerator.Current => Current;

    public ReverseListEnumerator(IList<T> list)
    {
        this = default(ReverseListEnumerator<T>);
        _list = list;
        Reset();
    }

    public void RemoveCurrent()
    {
        _list.RemoveAt(_currentIndex);
    }

    public bool MoveNext()
    {
        _currentIndex--;
        return _currentIndex >= 0;
    }

    public void Reset()
    {
        _currentIndex = _list.Count;
    }

    public void Dispose()
    {
    }
}
