using System;

using Mokus2D.Interfaces;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJour.Gameplay;

public abstract class CosPropertyChanger : IUpdatable
{
    private readonly CosChanger changer;

    protected Node Target { get; set; }

    public virtual float Value => changer.Value;

    public CosPropertyChanger(Node target, float minValue, float maxValue, float step)
    {
        Target = target;
        changer = new CosChanger(minValue, maxValue, step)
        {
            Progress = Maths.Random(0f, (float)Math.PI * 2f)
        };
    }

    public void Update(float time)
    {
        changer.Update(time);
        SetPropertyValue(Value);
    }

    protected abstract void SetPropertyValue(float value);
}
