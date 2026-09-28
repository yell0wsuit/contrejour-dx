using System;

using Mokus2D.Effects.Tweening;
using Mokus2D.Visual;

namespace Default.Namespace;

public class Tablo : Sprite
{
    private const float EFFECT_TIME = 0.2f;

    protected bool open;

    public bool Open
    {
        get => open;
        set
        {
            if (value != open)
            {
                open = value;
                Tweener.Stop();
                float targetValue = open ? 1 : 0;
                float targetValue2 = open ? 0f : ((float)Math.PI / 2f);
                TweenObject tweenObject = Tweener.Start(0.2f).Tween(NodeValues.Scale, targetValue).Tween(NodeValues.RotationRadians, targetValue2);
                if (open)
                {
                    Visible = true;
                }
                else
                {
                    _ = tweenObject.OnComplete(NodeValues.Hide);
                }
            }
        }
    }

    public Tablo(string clipName)
        : base(clipName)
    {
        Scale = 0f;
        RotationDegrees = -90f;
    }

    public Tablo()
        : this("planets/McPlanetTablo")
    {
    }
}
