using Microsoft.Xna.Framework;

namespace ContreJour.Gameplay;

public class FlyEye(ContreJourGame game, bool visible, Vector2 position) : MonsterEye(game, visible, position)
{
    protected override float ViewRadius => 4f;

    protected override void CreateDefaultView()
    {
        base.CreateDefaultView();
        EyeBallSprite.Scale = 1.5f;
    }
}
