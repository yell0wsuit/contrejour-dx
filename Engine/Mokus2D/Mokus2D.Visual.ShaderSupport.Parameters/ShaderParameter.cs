using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Effects.Tweening;
using Mokus2D.Util;

namespace Mokus2D.Visual.ShaderSupport.Parameters;

public abstract class ShaderParameter<T>(EffectParameter parameter)
{
    public static readonly GetSetValue<ShaderParameter<T>, T> GetSet = new(o => o.Value, delegate (ShaderParameter<T> o, T v)
    {
        o.Value = v;
    });

    protected readonly EffectParameter Parameter = parameter;
    private readonly Flag _valueDirty = new();

    public T Value
    {
        get
        {
            if (_valueDirty.Use())
            {
                field = GetValue();
            }
            return field;
        }
        set
        {
            field = value;
            SetValue(value);
        }
    }

    protected ShaderParameter(EffectParameterCollection parameters, string name)
        : this(parameters[name])
    {
    }

    public void Refresh()
    {
        SetValue(Value);
    }

    protected abstract void SetValue(T value);

    protected abstract T GetValue();
}
