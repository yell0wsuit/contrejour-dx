using System.Diagnostics.CodeAnalysis;

using Mokus2D.Interfaces;

namespace Default.Namespace;

public class WindManager : IUpdatable
{
    protected float windValue;

    protected float windChange;

    protected float currentWind;

    protected float currentWindStep;

    protected float step;

    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Its random draws must run after the base constructor's, in this order.")]
    public WindManager(float step)
    {
        this.step = step;
        currentWind = 0f;
        currentWindStep = 0f;
        windValue = GetRandomValue();
        windChange = GetRandomValue();
    }

    public float GetWind(float diff)
    {
        return (Maths.Sin(currentWindStep + diff) + 1f) / 2f * windValue;
    }

    public void Update(float time)
    {
        windChange += Maths.Random() * 0.01f;
        windValue = Maths.Cos(windChange);
        currentWindStep += step;
    }

    public static float GetRandomValue()
    {
        return Maths.Random(-1f, 1f);
    }
}
