using Microsoft.Xna.Framework;

using Mokus2D.Input;

namespace ContreJour.Gameplay;

public class TouchPositionProvider(Touch touch, LevelBuilderBase builder) : IVectorPositionProvider
{
    private readonly LevelBuilderBase builder = builder;

    public Touch Touch { get; } = touch;

    public Vector2 PositionVec => builder.TouchRootVec(Touch);
}
