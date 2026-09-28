using System;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace ContreJourMono.ContreJour.Game.Hero;

public class PointAndAngle
{
    private const float FAWN_MIN_STEP = 0.2f;

    private const float FAWN_STEP = 0.3f;

    public float Angle;

    private readonly float angleStep;

    private readonly float length;

    private Vector2 position;

    private readonly float angleOffset;

    private readonly float amplitude = 4f;

    private float fawnProgress;

    public float AngleStep => angleStep;

    public Vector2 Position => VectorExtensions.Rotate(position, Angle);

    public PointAndAngle(float length, float angleStep, float angleOffset)
    {
        position = new Vector2(length * 0.5f, 0f);
        this.length = length;
        this.angleStep = angleStep;
        this.angleOffset = angleOffset;
    }

    public void Update(float speed, bool onGround, float timeCoeff)
    {
        float target = speed.Clamp(0.5f, 1.1f) * length;
        position.X = position.X.StepTo(target, timeCoeff);
        position.Y = amplitude * (float)Math.Cos(fawnProgress + angleOffset);
        if (onGround)
        {
            fawnProgress += Math.Max(0.3f * speed, 0.2f * timeCoeff);
        }
        else
        {
            fawnProgress += 0.3f * timeCoeff;
        }
    }
}
