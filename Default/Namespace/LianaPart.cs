using System;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Util.MathUtils;

namespace Default.Namespace;

public class LianaPart : IUpdatable
{
    protected Body body;

    protected CosChanger forceChanger;

    protected float forceAngle;

    public LianaPart(Body _body)
    {
        body = _body;
        float num = Maths.Random(0.05f, 0.1f) * body.Mass;
        forceChanger = new CosChanger(0f - num, num, Maths.Random(0.01f, 0.02f));
        forceAngle = Maths.Random(0f, (float)Math.PI * 2f);
        body.GravityScale = 0f;
    }

    public void Update(float time)
    {
        forceChanger.Update(time);
        Vector2 vector = VectorUtil.ToVector(forceChanger.Value, forceAngle);
        body.ApplyForce(vector, body.WorldCenter);
        FarseerUtil.LimitSpeed(body, 0.3f);
    }
}
