using System;

using Mokus2D.Effects.Tweening;
using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class Tablo : Sprite
    {
        public bool Open
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Tweener.Stop();
                    float targetValue = field ? 1 : 0;
                    float targetValue2 = field ? 0f : ((float)Math.PI / 2f);
                    TweenObject tweenObject = Tweener.Start(0.2f).Tween(NodeValues.Scale, targetValue).Tween(NodeValues.RotationRadians, targetValue2);
                    if (field)
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
}
