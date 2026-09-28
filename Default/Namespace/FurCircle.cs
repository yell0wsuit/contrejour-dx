using System;
using Microsoft.Xna.Framework;
using Mokus2D.Util.MathUtils;

namespace Default.Namespace;

public class FurCircle : ParticleSystem
{
    protected float radius;

    protected float angleStep;

    public float Radius
    {
        get
        {
            return radius;
        }
        set
        {
            if (Maths.FuzzyNotEquals(radius, value))
            {
                radius = value;
                for (int i = 0; i < base.Particles.Count; i++)
                {
                    float itemAngle = GetItemAngle(i);
                    Particle particle = base.Particles[i];
                    particle.RotationDegrees = MathHelper.ToDegrees(itemAngle) - 90f;
                    particle.Position = VectorUtil.ToVector(value, itemAngle);
                }
            }
        }
    }

    public float AngleStep => angleStep;

    public FurCircle(string textureName, int maxParticles, float _radius)
        : base(textureName, maxParticles)
    {
        angleStep = 1f / (float)maxParticles * 2f * (float)Math.PI;
        Radius = _radius;
    }

    public float GetItemAngle(int i)
    {
        return (float)i * angleStep;
    }
}
