using System;
using System.Diagnostics.CodeAnalysis;

using Mokus2D.Interfaces;
using Mokus2D.Util.MathUtils;

namespace Default.Namespace;

public class Bouncer : IUpdatable
{
    protected float amplitude;

    protected float currentAmplitude;

    protected float amplitudeStep;

    protected CosChanger changer;

    public float Amplitude
    {
        get => amplitude;
        set => amplitude = value;
    }

    public float CurrentAmplitude => currentAmplitude;

    public float AmplitudeStep
    {
        get => amplitudeStep;
        set => amplitudeStep = value;
    }

    public float Step
    {
        get => changer.Step;
        set => changer.Step = value;
    }

    public float Value => changer.Value * currentAmplitude;

    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Its random draws must run after the base constructor's, in this order.")]
    public Bouncer(float _amplitude, float _amplitudeStep, float step)
    {
        changer = new CosChanger(-1f, 1f, step);
        amplitude = _amplitude;
        amplitudeStep = _amplitudeStep;
    }

    public void Start()
    {
        currentAmplitude = amplitude;
        changer.Progress = (float)Math.PI / 2f;
    }

    public void Update(float time)
    {
        if (Maths.FuzzyNotEquals(currentAmplitude, 0f))
        {
            currentAmplitude = Maths.StepTo(currentAmplitude, 0f, amplitudeStep * time);
            changer.Update(time);
        }
    }
}
