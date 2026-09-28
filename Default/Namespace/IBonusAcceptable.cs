using Microsoft.Xna.Framework;

namespace Default.Namespace;

public interface IBonusAcceptable : IBodyClip
{
    void ApplyBonus();

    Vector2 BonusTarget();
}
