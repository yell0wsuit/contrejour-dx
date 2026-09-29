using Mokus2D.Util.MathUtils;

namespace ContreJour.Gameplay
{
    public class WindData
    {
        public float MinAngle { get; }

        public float MaxAngle { get; }

        public float WindOffset { get; set; }

        public float Diff { get; }

        public WindData(float angle)
        {
            MinAngle = Maths.Random(0f - angle, 0f);
            MaxAngle = Maths.Random(0f, angle);
            Diff = MaxAngle - MinAngle;
            WindOffset = Maths.Random(-0.7f, 0.7f);
        }

        public float GetAngle(float wind)
        {
            return MinAngle + (Diff * wind);
        }
    }
}
