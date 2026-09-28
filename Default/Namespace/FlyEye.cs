using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class FlyEye(ContreJourGame _game, bool _visible, Vector2 position) : MonsterEye(_game, _visible, position)
{
    protected override float ViewRadius => 4f;

    protected override void CreateDefaultView()
    {
        base.CreateDefaultView();
        eyeBall.Scale = 1.5f;
    }
}
