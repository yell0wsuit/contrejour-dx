using System;

using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;

namespace Default.Namespace;

public class RotatorGrass
{
    protected CosChanger rotationChanger;

    protected float initialAngle;

    protected float initialDegrees;

    protected float contactAngle;

    protected float currentContactAngle;

    protected Particle particle;

    public float InitialAngle
    {
        get => initialAngle;
        set
        {
            initialAngle = value;
            initialDegrees = MathHelper.ToDegrees(value) - 90f;
            particle.RotationDegrees = initialDegrees;
        }
    }

    public float ContactAngle
    {
        get => contactAngle;
        set => contactAngle = value;
    }

    public Particle Particle
    {
        get => particle;
        set => particle = value;
    }

    public RotatorGrass(Particle particle)
    {
        rotationChanger = new CosChanger(-15f, 15f, Maths.Random(0.005f, 0.01f))
        {
            Progress = Maths.Random(0f, (float)Math.PI * 2f)
        };
        this.particle = particle;
        contactAngle = 0f;
        currentContactAngle = 0f;
    }

    public void UpdateAngle(float time, float angle)
    {
        rotationChanger.Update(time);
        currentContactAngle = Math.Abs(currentContactAngle) > Math.Abs(contactAngle) ? Maths.StepTo(currentContactAngle, contactAngle, 0.05f) : contactAngle;
        float num = initialDegrees + angle + rotationChanger.Value + MathHelper.ToDegrees(currentContactAngle);
        float maxStep = Math.Min(Math.Abs(particle.RotationDegrees - num) / 7f, 3f);
        particle.RotationDegrees = Maths.StepTo(particle.RotationDegrees, num, maxStep);
    }
}
