using System;

using Mokus2D.Util.MathUtils;

namespace ContreJour.Gameplay
{
    public class FurCircle : ParticleSystem
    {
        public float Radius
        {
            get;
            set
            {
                if (Maths.FuzzyNotEquals(field, value))
                {
                    field = value;
                    for (int i = 0; i < Particles.Count; i++)
                    {
                        float itemAngle = GetItemAngle(i);
                        Particle particle = Particles[i];
                        particle.RotationDegrees = float.RadiansToDegrees(itemAngle) - 90f;
                        particle.Position = VectorUtil.ToVector(value, itemAngle);
                    }
                }
            }
        }

        public float AngleStep { get; }

        public FurCircle(string textureName, int maxParticles, float radius)
            : base(textureName, maxParticles)
        {
            AngleStep = 1f / maxParticles * 2f * (float)Math.PI;
            Radius = radius;
        }

        public float GetItemAngle(int i)
        {
            return i * AngleStep;
        }
    }
}
