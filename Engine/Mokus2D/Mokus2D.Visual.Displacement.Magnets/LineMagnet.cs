using System;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace Mokus2D.Visual.Displacement.Magnets;

public class LineMagnet : GridMagnetBase
{
    private float _maxDistance;

    private Vector2 _start;

    private Vector2 _end;

    public Vector2 Start
    {
        get => _start;
        set
        {
            if (_start != value)
            {
                _start = value;
                RefreshBounds();
            }
        }
    }

    public Vector2 End
    {
        get => _end;
        set
        {
            if (_end != value)
            {
                _end = value;
                RefreshBounds();
            }
        }
    }

    public float MaxDistance
    {
        get => _maxDistance;
        set
        {
            if (_maxDistance != value)
            {
                _maxDistance = value;
                RefreshBounds();
            }
        }
    }

    public LineMagnet(Vector2 start, Vector2 end, float maxDistance, float power)
        : base(power)
    {
        Start = start;
        End = end;
        MaxDistance = maxDistance;
    }

    public void SetHeight(float value)
    {
        Start = new Vector2(0f, (0f - value) / 2f);
        End = new Vector2(0f, value / 2f);
    }

    public void SetWidth(float value)
    {
        Start = new Vector2((0f - value) / 2f, 0f);
        End = new Vector2(value / 2f, 0f);
    }

    public override Vector2 GetForce(Vector2 relativePosition)
    {
        Vector2 closestPoint = relativePosition.GetClosestPoint(Start, End);
        float num = closestPoint.DistanceTo(relativePosition);
        if (num >= _maxDistance)
        {
            return Vector2.Zero;
        }
        return relativePosition == closestPoint
            ? Vector2.Zero
            : (relativePosition - closestPoint).Normalize((MaxDistance - num) / MaxDistance * Power);
    }

    private void RefreshBounds()
    {
        Vector2 value = new(Math.Min(Start.X, End.X), Math.Min(Start.Y, End.Y));
        Vector2 value2 = (End - Start).Abs();
        value -= new Vector2(_maxDistance);
        value2 += new Vector2(_maxDistance) * 2f;
        value2 = VectorExtensions.Ceiling(value2);
        value = VectorExtensions.Floor(value);
        Bounds = new Rectangle((int)value.X, (int)value.Y, (int)value2.X, (int)value2.Y);
    }
}
