using System;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Util.MathUtils;

namespace ContreJour.Gameplay;

public class LianaPart : IUpdatable
{
    private readonly Body body;

    private readonly CosChanger forceChanger;

    private readonly float forceAngle;

    public LianaPart(Body body)
    {
        this.body = body;
        float num = Maths.Random(0.05f, 0.1f) * this.body.Mass;
        forceChanger = new CosChanger(0f - num, num, Maths.Random(0.01f, 0.02f));
        forceAngle = Maths.Random(0f, (float)Math.PI * 2f);
        this.body.GravityScale = 0f;
    }

    public void Update(float time)
    {
        forceChanger.Update(time);
        Vector2 vector = VectorUtil.ToVector(forceChanger.Value, forceAngle);
        body.ApplyForce(vector, body.WorldCenter);
        FarseerUtil.LimitSpeed(body, 0.3f);
    }
}
