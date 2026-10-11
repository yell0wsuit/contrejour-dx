using ContreJourDX.Utils;

namespace ContreJourDX.Gameplay
{
    public class ProgressLabel(float size, string format, int value, int steps) : ContreJourDXLabel(size)
    {
        private readonly string format = format;
        private readonly int steps = steps;

        private int currentStep;

        public int Value { get; set; } = value;

        public int CurrentValue => (int)(Value * (float)currentStep / steps);

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
}
