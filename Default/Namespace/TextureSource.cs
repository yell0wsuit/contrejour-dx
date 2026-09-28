namespace Default.Namespace;

internal class TextureSource
{
    private string path;

    private float textureScaleFactor;

    private int neededWidth;

    public string Path => path;

    public float TextureScaleFactor => textureScaleFactor;

    public int NeededWidth => neededWidth;

    public TextureSource(string path, float textureScaleFactor, int neededWidth)
    {
        this.path = path;
        this.textureScaleFactor = textureScaleFactor;
        this.neededWidth = neededWidth;
    }
}
