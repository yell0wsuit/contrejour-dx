using System.Collections.Generic;

using ContreJour.Primitives;

using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Util.Data;

namespace Default.Namespace;

public class SuckerNeckSprite : LongNeckSprite
{
    private Pair<Vector2> start;

    private Pair<Vector2> middle;

    private Pair<Vector2> end;

    private readonly Bouncer bouncer;
    private int frame;

    public float Length
    {
        get;
        set
        {
            if (Maths.FuzzyNotEquals(field, value))
            {
                field = value;
                RefreshMiddle();
                end = new Pair<Vector2>(new Vector2(field, 9f), new Vector2(field, -9f));
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

    public static new Color EndColor()
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
        middle = new Pair<Vector2>(new Vector2(Length / 2f, -1f + (bouncer.CurrentAmplitude * num)), new Vector2(Length / 2f, 1f + (bouncer.CurrentAmplitude * num)));
    }

    public override void GetPairs(List<Pair<Vector2>> target)
    {
        target.Add(start);
        target.Add(middle);
        target.Add(end);
    }
}
