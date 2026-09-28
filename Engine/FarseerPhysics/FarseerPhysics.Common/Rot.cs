using System;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common;

public struct Rot(float angle)
{
    public float s = (float)Math.Sin(angle);

    public float c = (float)Math.Cos(angle);

    public void Set(float angle)
    {
        s = (float)Math.Sin(angle);
        c = (float)Math.Cos(angle);
    }

    public void SetIdentity()
    {
        s = 0f;
        c = 1f;
    }

    public float GetAngle()
    {
        return (float)Math.Atan2(s, c);
    }

    public Vector2 GetXAxis()
    {
        return new Vector2(c, s);
    }

    public Vector2 GetYAxis()
    {
        return new Vector2(0f - s, c);
    }
}
