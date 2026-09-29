namespace Mokus2D.Util.MathUtils
{
    public static class Comparisons
    {
        public const int Less = -1;

        public const int Greater = 1;

        public const int Equal = 0;

        public static int Reverse(int result)
        {
            return result < 0 ? 1 : result > 0 ? -1 : 0;
        }

        public static int IntReverseComparizon(int first, int second)
        {
            return Reverse(IntComparizon(first, second));
        }

        public static int LongComparizon(long first, long second)
        {
            return first > second ? 1 : second > first ? -1 : 0;
        }

        public static int IntComparizon(int first, int second)
        {
            return LongComparizon(first, second);
        }

        public static int FloatComparizon(float first, float second)
        {
            return first > second ? 1 : second > first ? -1 : 0;
        }

        public static int DoubleComparizon(double first, double second)
        {
            return first > second ? 1 : second > first ? -1 : 0;
        }
    }
}
