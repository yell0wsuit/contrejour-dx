using System;
using Microsoft.Xna.Framework;
using Mokus2D.Util;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class BouncingSprite : Sprite
{
    protected CosChanger changer;

    protected float initialScale;

    public float Step
    {
        get
        {
            return changer.Step;
        }
        set
        {
            changer.Step = value;
        }
    }

    public override Vector2 ScaleVec
    {
        set
        {
            base.ScaleVec = value;
            initialScale = value.X;
        }
    }

    public event Action<BouncingSprite> MaxBounceEvent;

    public BouncingSprite(string filename)
        : base(filename)
    {
        changer = new CosChanger(0.03f, 0.05f);
        changer.MinValue = 0.95f;
        changer.MaxValue = 1.04f;
        initialScale = 1f;
    }

    public override void Update(float time)
    {
        changer.Update(time);
        base.ScaleVec = new Vector2(changer.Value * initialScale, (2f - changer.Value) * initialScale);
        if (changer.IsMax)
        {
            this.MaxBounceEvent.Dispatch(this);
        }
    }
}
