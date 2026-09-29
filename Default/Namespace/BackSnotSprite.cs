namespace Default.Namespace;

public class BackSnotSprite : SnotSprite
{
    public BackSnotSprite(SnotBodyClipBase snot, float startWidth, float centerWidth, float endWidth)
        : base(snot, startWidth, centerWidth, endWidth)
    {
        BorderWidth = 10f;
    }
}
