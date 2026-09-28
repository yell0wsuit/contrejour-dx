using System;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common;

public struct Sweep
{
    public float A;

    public float A0;

    public float Alpha0;

    public Vector2 C;

    public Vector2 C0;

    public Vector2 LocalCenter;

    public readonly void GetTransform(out Transform xfb, float beta)
    {
        xfb = default;
        xfb.p.X = ((1f - beta) * C0.X) + (beta * C.X);
        xfb.p.Y = ((1f - beta) * C0.Y) + (beta * C.Y);
        float angle = ((1f - beta) * A0) + (beta * A);
        xfb.q.Set(angle);
        xfb.p -= MathUtils.Mul(xfb.q, LocalCenter);
    }

    public void Advance(float alpha)
    {
        float num = (alpha - Alpha0) / (1f - Alpha0);
        C0 += num * (C - C0);
        A0 += num * (A - A0);
        Alpha0 = alpha;
    }

    public void Normalize()
    {
        float num = (float)Math.PI * 2f * (float)Math.Floor(A0 / ((float)Math.PI * 2f));
        A0 -= num;
        A -= num;
    }
}
