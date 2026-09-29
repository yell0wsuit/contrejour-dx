using Microsoft.Xna.Framework;

using Mokus2D.Input;

namespace Default.Namespace;

public class TouchPositionProvider(Touch _touch, LevelBuilderBase _builder) : IVectorPositionProvider
{
    private readonly LevelBuilderBase builder = _builder;

    public Touch Touch { get; } = _touch;

    public Vector2 PositionVec => builder.TouchRootVec(Touch);
}
