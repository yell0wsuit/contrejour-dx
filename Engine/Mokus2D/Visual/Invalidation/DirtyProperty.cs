using System;

using Mokus2D.Util;

namespace Mokus2D.Visual.Invalidation;

public struct DirtyProperty<T> where T : IEquatable<T>
{
    private T _value;
    private readonly Action<T> _refreshMethod;

    public T Value
    {
        readonly get => _value;
        set
        {
            if (!_value.Equals(value))
            {
                _value = value;
                Dirty = true;
            }
        }
    }

    public bool Dirty { get; private set; }

    public DirtyProperty(T value)
    {
        this = default;
        _value = value;
        Dirty = true;
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
        Dirty = true;
    }

    public void SetClean()
    {
        Dirty = false;
    }
}
