using Microsoft.Xna.Framework;

using Mokus2D.Integration.Farseer.Physics;

namespace ContreJour.Gameplay;

public interface IEatable : IBodyClip
{
    void EatSpeedPauseScaleTime(Vector2 targetPosition, float finishSpeed, float pause, float scale, float time);

    float DeadEyeScale();

    bool CanDie();
}
