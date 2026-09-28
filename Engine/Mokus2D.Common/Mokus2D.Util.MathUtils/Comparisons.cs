namespace Mokus2D.Util.MathUtils;

public static class Comparisons
{
    public const int Less = -1;

    public const int Greater = 1;

    public const int Equal = 0;

    public static int Reverse(int result)
    {
        if (result < 0)
        {
            return 1;
        }
        if (result > 0)
        {
            return -1;
        }
        return 0;
    }

    public static int IntReverseComparizon(int first, int second)
    {
        return Reverse(IntComparizon(first, second));
    }

    public static int LongComparizon(long first, long second)
    {
        if (first > second)
        {
            return 1;
        }
        if (second > first)
        {
            return -1;
        }
        return 0;
    }

    public static int IntComparizon(int first, int second)
    {
        return LongComparizon(first, second);
    }

    public static int FloatComparizon(float first, float second)
    {
        if (first > second)
        {
            return 1;
        }
        if (second > first)
        {
            return -1;
        }
        return 0;
    }

    public static int DoubleComparizon(double first, double second)
    {
        if (first > second)
        {
            return 1;
        }
        if (second > first)
        {
            return -1;
        }
        return 0;
    }
}
