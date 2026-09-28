using System;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Default.Namespace;

public class AccelerometerNode : Node
{
    protected Vector2 accelerometerOffset;

    protected bool accelerometerUsed;

    protected Vector2 maxAccOffset = new((float)Math.PI / 22f, 80f);

    // Desktop has no accelerometer; behaves like the original when Accelerometer.GetDefault() returned null.
    private static Vector2 Acceleration => Vector2.Zero;

    private void UpdateOffset(Vector2 acceleration)
    {
        Vector2 vector = default;
        vector.X = Maths.Clamp((0f - acceleration.Y) * 3f * maxAccOffset.X, 0f - maxAccOffset.X, maxAccOffset.X);
        vector.Y = Maths.Clamp((acceleration.X + 0.7f) * maxAccOffset.Y * 2f, 0f - maxAccOffset.Y, maxAccOffset.Y);
        if (!accelerometerUsed)
        {
            accelerometerOffset = vector;
            accelerometerUsed = true;
        }
        else
        {
            accelerometerOffset = VectorExtensions.StepTo(step: ((vector - accelerometerOffset) * 0.015f).Abs(), source: accelerometerOffset, target: vector);
        }
    }

    public override void Update(float time)
    {
        Vector2 acceleration = Acceleration;
        if (acceleration.X > 0f)
        {
            acceleration.X *= -1f;
        }
        UpdateOffset(acceleration);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}
