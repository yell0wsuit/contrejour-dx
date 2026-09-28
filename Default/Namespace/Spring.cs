using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class Spring
{
    public Body bodyA;

    public Body bodyB;

    public float k;

    public Vector2 localAnchorA = default(Vector2);

    public Vector2 localAnchorB = default(Vector2);

    public Spring(ref Body bodyA, ref Body bodyB, Vector2 anchorA, Vector2 anchorB, float k)
    {
        this.bodyA = bodyA;
        this.bodyB = bodyB;
        this.k = k;
        localAnchorA = bodyA.GetLocalPoint(anchorA);
        localAnchorB = bodyB.GetLocalPoint(anchorB);
    }

    public void update(float time)
    {
        //IL_0006: Unknown result type (might be due to invalid IL or missing references)
        //IL_0013: Unknown result type (might be due to invalid IL or missing references)
        //IL_0093: Unknown result type (might be due to invalid IL or missing references)
        //IL_00b6: Unknown result type (might be due to invalid IL or missing references)
        if ((int)bodyA.BodyType == 0 && (int)bodyB.BodyType == 0)
        {
            return;
        }
        Vector2 worldPoint = bodyA.GetWorldPoint(localAnchorA);
        Vector2 worldPoint2 = bodyB.GetWorldPoint(localAnchorB);
        Vector2 vector = worldPoint2 - worldPoint;
        float num = vector.Length();
        vector *= 1f / num;
        Vector2 vector2 = vector;
        vector2 *= -1f;
        float num2 = k * num * num;
        if (Maths.FuzzyNotEquals(num2, 0f))
        {
            if ((int)bodyA.BodyType != 0)
            {
                vector *= num2;
                bodyA.ApplyForce(vector, worldPoint);
            }
            if ((int)bodyB.BodyType != 0)
            {
                vector2 *= num2;
                bodyB.ApplyForce(vector2, worldPoint2);
            }
        }
    }
}
