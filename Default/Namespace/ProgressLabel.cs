using ContreJour.Utils;

namespace Default.Namespace;

public class ProgressLabel : ContreJourLabel
{
    protected string format;

    protected int value;

    protected int steps;

    protected int currentStep;

    public int Value
    {
        get
        {
            return value;
        }
        set
        {
            this.value = value;
        }
    }

    public int CurrentValue => (int)((float)value * (float)currentStep / (float)steps);

    public ProgressLabel(float size, string _format, int _value, int _steps)
        : base(size)
    {
        value = _value;
        format = _format;
        steps = _steps;
        currentStep = 0;
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (currentStep <= steps)
        {
            Clear();
            AppendFormat(format, CurrentValue);
            currentStep++;
        }
    }
}
