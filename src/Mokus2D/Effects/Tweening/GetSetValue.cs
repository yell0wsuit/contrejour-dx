using System;

namespace Mokus2D.Effects.Tweening
{
    public class GetSetValue<TValue>(Func<object, TValue> getter, Action<object, TValue> setter)
    {
        private readonly Func<object, TValue> _getter = getter;

        private readonly Action<object, TValue> _setter = setter;

        public TValue GetValue(object node)
        {
            return _getter(node);
        }

        public void SetValue(object node, TValue value)
        {
            _setter(node, value);
        }
    }
    public class GetSetValue<TObject, TValue>(Func<TObject, TValue> getter, Action<TObject, TValue> setter) : GetSetValue<TValue>(o => getter((TObject)o), delegate (object o, TValue v)
            {
                setter((TObject)o, v);
            }) where TObject : class
    {
        private readonly Func<TObject, TValue> _getter = getter;

        private readonly Action<TObject, TValue> _setter = setter;

        public TValue GetValue(TObject node)
        {
            return _getter(node);
        }

        public void SetValue(TObject node, TValue value)
        {
            _setter(node, value);
        }
    }
}
