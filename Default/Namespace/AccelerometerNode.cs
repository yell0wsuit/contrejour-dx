using System;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Default.Namespace;

public class AccelerometerNode : Node
{
    private const float StepCoeff = 0.015f;

    private const float MinChange = 0.05f;

    private const float SpeedStep = 0.05f;

    protected Vector2 accelerometerOffset;

    protected bool accelerometerUsed;

    protected Vector2 maxAccOffset = new Vector2((float)Math.PI / 22f, 80f);

    private Vector2 speed = Vector2.Zero;

    // Desktop has no accelerometer; behaves like the original when Accelerometer.GetDefault() returned null.
    private Vector2 Acceleration => Vector2.Zero;

    private void UpdateOffset(Vector2 acceleration)
    {
        Vector2 vector = default(Vector2);
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
