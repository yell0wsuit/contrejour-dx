using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Default.Namespace;

public class FlowerEye : MonsterEye
{
    protected Node baseNode;

    protected Vector2 initialPosition;

    public override Vector2 Position
    {
        set
        {
            base.Position = value;
            initialPosition = value;
        }
    }

    public FlowerEye(ContreJourGame game, bool visible, Vector2 position)
        : base(game, visible, position)
    {
        baseNode = new Sprite(game.Choose("common/McFlowerHead", null, "chapter4/McFlowerHeadWhite", null, "chapter6/McFlowerHead_6"));
        if (!game.WhiteSide)
        {
            Scale = 0.85f;
        }
        AddChild(baseNode, -1);
    }

    public override void Update(float time)
    {
        base.Update(time);
        base.Position = initialPosition + (currentEyeBall.Position * 2f);
    }
}
