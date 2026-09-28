namespace ContreJour.Config;

public readonly struct AspectRatio(float ratio, string levelsFolder)
{
    public static readonly AspectRatio Ratio5x3 = new(1.6666666f, "");

    public static readonly AspectRatio Ratio16x9 = new(1.7777778f, "16X9");

    public static readonly AspectRatio[] All =
    [
        new(1.4222223f, "4X3"),
        Ratio5x3,
        Ratio16x9
    ];

    public readonly float Ratio { get; } = ratio;

    public readonly string LevelsFolder { get; } = levelsFolder;
}
