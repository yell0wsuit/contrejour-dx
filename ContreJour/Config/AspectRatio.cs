namespace ContreJour.Config;

public struct AspectRatio(float ratio, string levelsFolder)
{
    public static readonly AspectRatio Ratio5x3 = new(1.6666666f, "");

    public static readonly AspectRatio Ratio16x9 = new(1.7777778f, "16X9");

    public static readonly AspectRatio[] All =
    [
        new(1.4222223f, "4X3"),
        Ratio5x3,
        Ratio16x9
    ];

    private float ratio = ratio;

    private string levelsFolder = levelsFolder;

    public readonly float Ratio => ratio;

    public readonly string LevelsFolder => levelsFolder;
}
