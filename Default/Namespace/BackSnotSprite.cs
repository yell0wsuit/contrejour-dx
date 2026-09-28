namespace Default.Namespace;

public class BackSnotSprite : SnotSprite
{
    public BackSnotSprite(SnotBodyClipBase _snot, float _startWidth, float _centerWidth, float _endWidth)
        : base(_snot, _startWidth, _centerWidth, _endWidth)
    {
        borderWidth = 10f;
    }
}
