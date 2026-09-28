using System;

using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;

namespace Default.Namespace;

public class MovingRotatingSprite : RotatingSprite
{
    protected CosChanger changer;

    protected Vector2 initialPosition;

    public override Vector2 Position
    {
        set
        {
            base.Position = value;
            initialPosition = value;
        }
    }

    public MovingRotatingSprite(string filename)
        : base(filename)
    {
        changer = new CosChanger(0f, 0f);
    }

    public override void Update(float time)
    {
        base.Update(time);
        changer.Update(time);
        base.Position = initialPosition + new Vector2(changer.Value, 0f);
    }

    public void Initialize(float amplitude, float progress)
    {
        changer.MinValue = 0f - amplitude;
        changer.MaxValue = amplitude;
        changer.Step = (float)Math.PI;
        changer.Progress = progress;
    }
}
