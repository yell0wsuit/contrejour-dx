using ContreJour.Utils;

namespace Default.Namespace;

public class ProgressLabel(float size, string _format, int _value, int _steps) : ContreJourLabel(size)
{
    protected string format = _format;

    protected int value = _value;

    protected int steps = _steps;

    protected int currentStep;

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
