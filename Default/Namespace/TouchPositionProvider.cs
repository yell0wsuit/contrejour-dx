using Microsoft.Xna.Framework;

using Mokus2D.Input;

namespace Default.Namespace;

public class TouchPositionProvider(Touch _touch, LevelBuilderBase _builder) : IVectorPositionProvider
{
    private LevelBuilderBase builder = _builder;

    private Touch touch = _touch;

    public Touch Touch => touch;

    public Vector2 PositionVec => builder.TouchRootVec(touch);
}
