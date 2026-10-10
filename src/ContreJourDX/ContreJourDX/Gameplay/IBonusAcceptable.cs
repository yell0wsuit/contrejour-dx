using System.Numerics;

using Mokus2D.Integration.Farseer.Physics;

namespace ContreJourDX.Gameplay
{
    public interface IBonusAcceptable : IBodyClip
    {
        void ApplyBonus();

        Vector2 BonusTarget();
    }
}
