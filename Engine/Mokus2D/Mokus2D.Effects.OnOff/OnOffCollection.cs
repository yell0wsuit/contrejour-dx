using System.Collections.Generic;

namespace Mokus2D.Effects.OnOff;

public class OnOffCollection : OnOffEffect
{
    private readonly List<IOnOff> _effects;

    public OnOffCollection(params IOnOff[] effects)
        : base(null)
    {
        _effects = [.. effects];
    }

    public void Add(IOnOff onOff)
    {
        _effects.Add(onOff);
    }

    protected override void SetOn()
    {
        foreach (IOnOff effect in _effects)
        {
            effect.IsOn = true;
        }
    }

    protected override void SetOff()
    {
        foreach (IOnOff effect in _effects)
        {
            effect.IsOn = false;
        }
    }

    public override void SetOn(bool value)
    {
        base.SetOn(value);
        foreach (IOnOff effect in _effects)
        {
            effect.SetOn(value);
        }
    }
}
