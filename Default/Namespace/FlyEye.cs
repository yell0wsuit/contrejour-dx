using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class FlyEye : MonsterEye
{
    protected override float ViewRadius => 4f;

    public FlyEye(ContreJourGame _game, bool _visible, Vector2 position)
        : base(_game, _visible, position)
    {
    }

    protected override void CreateDefaultView()
    {
        base.CreateDefaultView();
        eyeBall.Scale = 1.5f;
    }
}
