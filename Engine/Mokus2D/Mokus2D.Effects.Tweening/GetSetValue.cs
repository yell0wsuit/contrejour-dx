using System;

namespace Mokus2D.Effects.Tweening;

public class GetSetValue<TValue>
{
    private readonly Func<object, TValue> _getter;

    private readonly Action<object, TValue> _setter;

    public GetSetValue(Func<object, TValue> getter, Action<object, TValue> setter)
    {
        _getter = getter;
        _setter = setter;
    }

    public TValue GetValue(object node)
    {
        return _getter(node);
    }

    public void SetValue(object node, TValue value)
    {
        _setter(node, value);
    }
}
public class GetSetValue<TObject, TValue> : GetSetValue<TValue> where TObject : class
{
    private readonly Func<TObject, TValue> _getter;

    private readonly Action<TObject, TValue> _setter;

    public GetSetValue(Func<TObject, TValue> getter, Action<TObject, TValue> setter)
        : base((Func<object, TValue>)((object o) => getter((TObject)o)), (Action<object, TValue>)delegate (object o, TValue v)
        {
            setter((TObject)o, v);
        })
    {
        _getter = getter;
        _setter = setter;
    }

    public TValue GetValue(TObject node)
    {
        return _getter(node);
    }

    public void SetValue(TObject node, TValue value)
    {
        _setter(node, value);
    }
}
