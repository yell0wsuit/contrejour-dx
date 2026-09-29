using ContreJour.Utils;

namespace Default.Namespace;

public class ProgressLabel(float size, string _format, int _value, int _steps) : ContreJourLabel(size)
{
    private readonly string format = _format;

    private int value = _value;

    private readonly int steps = _steps;

    private int currentStep;

    public int Value
    {
        get => value;
        set => this.value = value;
    }

    public int CurrentValue => (int)(value * (float)currentStep / steps);

    public override void Update(float time)
    {
        base.Update(time);
        if (currentStep <= steps)
        {
            _ = Clear();
            _ = AppendFormat(format, CurrentValue);
            currentStep++;
        }
    }
}
