using System;

using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;

namespace Default.Namespace;

public class FurCircle : ParticleSystem
{
    private float radius;

    private readonly float angleStep;

    public float Radius
    {
        get => radius;
        set
        {
            if (Maths.FuzzyNotEquals(radius, value))
            {
                radius = value;
                for (int i = 0; i < Particles.Count; i++)
                {
                    float itemAngle = GetItemAngle(i);
                    Particle particle = Particles[i];
                    particle.RotationDegrees = MathHelper.ToDegrees(itemAngle) - 90f;
                    particle.Position = VectorUtil.ToVector(value, itemAngle);
                }
            }
        }
    }

    public float AngleStep => angleStep;

    public FurCircle(string textureName, int maxParticles, float radius)
        : base(textureName, maxParticles)
    {
        angleStep = 1f / maxParticles * 2f * (float)Math.PI;
        Radius = radius;
    }

    public float GetItemAngle(int i)
    {
        return i * angleStep;
    }
}
