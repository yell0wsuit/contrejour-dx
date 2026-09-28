namespace Default.Namespace;

public class WindData
{
    protected float minAngle;

    protected float maxAngle;

    protected float windOffset;

    protected float diff;

    public float MinAngle => minAngle;

    public float MaxAngle => maxAngle;

    public float WindOffset
    {
        get
        {
            return windOffset;
        }
        set
        {
            windOffset = value;
        }
    }

    public float Diff => diff;

    public WindData(float angle)
    {
        minAngle = Maths.Random(0f - angle, 0f);
        maxAngle = Maths.Random(0f, angle);
        diff = maxAngle - minAngle;
        windOffset = Maths.Random(-0.7f, 0.7f);
    }

    public float GetAngle(float wind)
    {
        return minAngle + diff * wind;
    }
}
