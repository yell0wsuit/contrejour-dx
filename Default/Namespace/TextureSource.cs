namespace Default.Namespace;

internal class TextureSource
{
    public string Path { get; }

    public float TextureScaleFactor { get; }

    public int NeededWidth { get; }

    public TextureSource(string path, float textureScaleFactor, int neededWidth)
    {
        Path = path;
        TextureScaleFactor = textureScaleFactor;
        NeededWidth = neededWidth;
    }
}
