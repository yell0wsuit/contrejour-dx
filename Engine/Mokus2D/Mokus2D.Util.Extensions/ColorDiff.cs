namespace Mokus2D.Util.Extensions;

public struct ColorDiff(int r, int g, int b, int a)
{
    public int R = r;

    public int G = g;

    public int B = b;

    public int A = a;

    public static ColorDiff operator *(ColorDiff source, float mult)
    {
        return new ColorDiff((int)((float)source.R * mult), (int)((float)source.G * mult), (int)((float)source.B * mult), (int)((float)source.A * mult));
    }
}
