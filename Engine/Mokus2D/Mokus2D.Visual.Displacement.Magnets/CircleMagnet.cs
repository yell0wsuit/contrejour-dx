using System;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace Mokus2D.Visual.Displacement.Magnets;

public class CircleMagnet : GridMagnetBase
{
    public float Radius
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                int num = field.Ceiling();
                Bounds = new Rectangle(-num, -num, num * 2, num * 2);
            }
        }
    }

    public CircleMagnet(float radius, float power)
        : this(radius)
    {
        Power = power;
    }

    public CircleMagnet(float radius)
    {
        Radius = radius;
    }

    public override Vector2 GetForce(Vector2 relativePosition)
    {
        float num = relativePosition.Length();
        if (num > Radius)
        {
            return Vector2.Zero;
        }
        float powerCoeff = (Radius - num) / Radius;
        return GetForce(relativePosition, powerCoeff, num);
    }

    protected virtual Vector2 GetForce(Vector2 relativePosition, float powerCoeff, float length)
    {
        float num = powerCoeff * Power;
        if (Power < 0f)
        {
            num = Math.Max(num, 0f - length);
        }
        num = LimitPower(num);
        return relativePosition.Normalize(num);
    }
}
