using System.Collections.Generic;

using ContreJour.Primitives;

using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Util.Data;

namespace Default.Namespace;

public class SuckerNeckSprite : LongNeckSprite
{
    private const float MIDDLE_WIDTH = -1f;

    private const float START_WIDTH = 9f;

    protected Pair<Vector2> start;

    protected Pair<Vector2> middle;

    protected Pair<Vector2> end;

    protected Bouncer bouncer;

    protected float length;

    private int frame;

    public float Length
    {
        get => length;
        set
        {
            if (Maths.FuzzyNotEquals(length, value))
            {
                length = value;
                RefreshMiddle();
                end = new Pair<Vector2>(new Vector2(length, 9f), new Vector2(length, -9f));
            }
        }
    }

    public SuckerNeckSprite()
    {
        start = new Pair<Vector2>(new Vector2(0f, 9f), new Vector2(0f, -9f));
        middle = start;
        end = start;
        bouncer = new Bouncer(6f, 5f, 5f);
        Effect = Mokus2DGame.Config.GraphicsConfig.DefaultEffect;
    }

    public override void Update(float time)
    {
        frame++;
        bouncer.Update(time);
        RefreshMiddle();
        base.Update(time);
    }

    public new Color EndColor()
    {
        return new Color(100, 100, 100, 0);
    }

    public virtual void LightBounce()
    {
        bouncer.Amplitude = 6f;
        bouncer.AmplitudeStep = 5f;
        bouncer.Step = 3f;
        bouncer.Start();
    }

    public virtual void Bounce()
    {
        bouncer.Amplitude = 9f;
        bouncer.AmplitudeStep = 4f;
        bouncer.Step = 3f;
        bouncer.Start();
    }

    public void RefreshMiddle()
    {
        float num = (frame % 4 > 1) ? 1 : (-1);
        middle = new Pair<Vector2>(new Vector2(length / 2f, -1f + (bouncer.CurrentAmplitude * num)), new Vector2(length / 2f, 1f + (bouncer.CurrentAmplitude * num)));
    }

    public override void GetPairs(List<Pair<Vector2>> target)
    {
        target.Add(start);
        target.Add(middle);
        target.Add(end);
    }
}
