using System;
using System.Numerics;

using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace ContreJourDX.Gameplay.Hero
{
    public class PointAndAngle(float length, float angleStep, float angleOffset)
    {
        public float Angle { get; set; }
        private readonly float length = length;

        private Vector2 position = new(length * 0.5f, 0f);

        private readonly float angleOffset = angleOffset;

        private readonly float amplitude = 4f;

        private float fawnProgress;

        public float AngleStep { get; } = angleStep;

        public Vector2 Position => VectorExtensions.Rotate(position, Angle);

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
}
