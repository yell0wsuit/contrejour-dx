using Microsoft.Xna.Framework;
using Mokus2D.Input;

namespace Default.Namespace;

public class TouchPositionProvider : IVectorPositionProvider
{
    protected LevelBuilderBase builder;

    protected Touch touch;

    public Touch Touch => touch;

    public Vector2 PositionVec => builder.TouchRootVec(touch);

    public TouchPositionProvider(Touch _touch, LevelBuilderBase _builder)
    {
        touch = _touch;
        builder = _builder;
    }
}
