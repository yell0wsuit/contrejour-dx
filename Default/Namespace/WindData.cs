namespace Default.Namespace;

public class WindData
{
    private float minAngle;

    private float maxAngle;

    private float windOffset;

    private float diff;

    public float MinAngle => minAngle;

    public float MaxAngle => maxAngle;

    public float WindOffset
    {
        get => windOffset;
        set => windOffset = value;
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
        return minAngle + (diff * wind);
    }
}
