using System;

using Mokus2D.Util;

namespace Mokus2D.Visual.Invalidation;

public struct DirtyProperty<T> where T : IEquatable<T>
{
    private T _value;

    private bool _dirty;

    private readonly Action<T> _refreshMethod;

    public T Value
    {
        get
        {
            return _value;
        }
        set
        {
            if (!_value.Equals(value))
            {
                _value = value;
                _dirty = true;
            }
        }
    }

    public bool Dirty => _dirty;

    public DirtyProperty(T value)
    {
        this = default(DirtyProperty<T>);
        _value = value;
        _dirty = true;
    }

    public DirtyProperty(T value, Action<T> refreshMethod)
        : this(value)
    {
        _refreshMethod = refreshMethod;
    }

    public bool TryRefresh()
    {
        if (Dirty)
        {
            SetClean();
            _refreshMethod.Dispatch(_value);
            return true;
        }
        return false;
    }

    public void SetDirty()
    {
        _dirty = true;
    }

    public void SetClean()
    {
        _dirty = false;
    }
}
