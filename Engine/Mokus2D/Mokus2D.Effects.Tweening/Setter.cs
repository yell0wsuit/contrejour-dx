namespace Mokus2D.Effects.Tweening;

public delegate void Setter<TObject, in TValue>(ref TObject source, TValue value);
