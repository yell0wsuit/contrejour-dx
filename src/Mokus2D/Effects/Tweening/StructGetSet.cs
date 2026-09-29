namespace Mokus2D.Effects.Tweening
{
    public class StructGetSet<TObject, TValue>(Getter<TObject, TValue> getter, Setter<TObject, TValue> setter) where TObject : struct
    {
        private readonly Getter<TObject, TValue> _getter = getter;

        private readonly Setter<TObject, TValue> _setter = setter;

        public TValue GetValue(TObject target)
        {
            return _getter(ref target);
        }

        public void SetValue(TObject target, TValue value)
        {
            _setter(ref target, value);
        }
    }
}
