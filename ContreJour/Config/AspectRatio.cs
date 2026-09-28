namespace ContreJour.Config;

public struct AspectRatio(float ratio, string levelsFolder)
{
    public static readonly AspectRatio Ratio5x3 = new AspectRatio(1.6666666f, "");

    public static readonly AspectRatio Ratio16x9 = new AspectRatio(1.7777778f, "16X9");

    public static readonly AspectRatio[] All = new AspectRatio[3]
    {
        new AspectRatio(1.4222223f, "4X3"),
        Ratio5x3,
        Ratio16x9
    };

    private float ratio = ratio;

    private string levelsFolder = levelsFolder;

    public float Ratio => ratio;

    public string LevelsFolder => levelsFolder;
}
