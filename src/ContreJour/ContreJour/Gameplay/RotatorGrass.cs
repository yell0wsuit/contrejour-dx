using System;

using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;

namespace ContreJour.Gameplay
{
    public class RotatorGrass
    {
        private readonly CosChanger rotationChanger;
        private float initialDegrees;
        private float currentContactAngle;

        public float InitialAngle
        {
            get; set
            {
                field = value;
                initialDegrees = MathHelper.ToDegrees(value) - 90f;
                Particle.RotationDegrees = initialDegrees;
            }
        }

        public float ContactAngle { get; set; }

        public Particle Particle { get; set; }

        public RotatorGrass(Particle particle)
        {
            rotationChanger = new CosChanger(-15f, 15f, Maths.Random(0.005f, 0.01f))
            {
                Progress = Maths.Random(0f, (float)Math.PI * 2f)
            };
            Particle = particle;
            ContactAngle = 0f;
            currentContactAngle = 0f;
        }

        public void UpdateAngle(float time, float angle)
        {
            rotationChanger.Update(time);
            currentContactAngle = Math.Abs(currentContactAngle) > Math.Abs(ContactAngle) ? Maths.StepTo(currentContactAngle, ContactAngle, 0.05f) : ContactAngle;
            float num = initialDegrees + angle + rotationChanger.Value + MathHelper.ToDegrees(currentContactAngle);
            float maxStep = Math.Min(Math.Abs(Particle.RotationDegrees - num) / 7f, 3f);
            Particle.RotationDegrees = Maths.StepTo(Particle.RotationDegrees, num, maxStep);
        }
    }
}
