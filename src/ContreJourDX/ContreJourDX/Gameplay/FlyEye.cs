using System.Numerics;

namespace ContreJourDX.Gameplay
{
    public class FlyEye(ContreJourDXGame game, bool visible, Vector2 position) : MonsterEye(game, visible, position)
    {
        protected override float ViewRadius => 4f;

        protected override void CreateDefaultView()
        {
            base.CreateDefaultView();
            EyeBallSprite.Scale = 1.5f;
        }
    }
}
