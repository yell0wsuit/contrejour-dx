using System;

using Default.Namespace;

using Microsoft.Xna.Framework;

namespace Mokus2D.Util.MathUtils;

public class CosChanger
{
    public float Progress;

    public float Step;

    public float MinValue;

    public float MaxValue;

    private float value;

    public float Value => value;

    public bool IsMax => Math.Abs(Progress % (Math.PI * 2.0)) < Step;

    public CosChanger(float minValue, float maxValue, float step, float? progressCoef = null)
    {
        Progress = (float)(((double?)progressCoef) ?? ((double)Maths.Random() * Math.PI));
        Step = step;
        MinValue = minValue;
        MaxValue = maxValue;
        Update(0f);
    }

    public CosChanger(float minStep, float maxStep)
        : this(0f, 1f, Maths.Random(minStep, maxStep))
    {
    }

    public CosChanger(float step)
        : this(0f, 1f, step)
    {
    }

    public void Update(float time)
    {
        Progress += Step * time;
        value = GetValue(MinValue, MaxValue, Progress);
    }

    public float GetValue(float phaseOffset)
    {
        return GetValue(MinValue, MaxValue, Progress + phaseOffset);
    }

    public static float GetValue(float min, float max, float p)
    {
        return MathHelper.Lerp(max, min, (float)(1.0 + Math.Cos(p)) / 2f);
    }

    public void SetMiddleProgress()
    {
        Progress = (float)Math.PI / 2f;
    }
}
