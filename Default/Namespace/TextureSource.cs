namespace Default.Namespace;

internal class TextureSource(string path, float textureScaleFactor, int neededWidth)
{
    public string Path { get; } = path;

    public float TextureScaleFactor { get; } = textureScaleFactor;

    public int NeededWidth { get; } = neededWidth;
}
