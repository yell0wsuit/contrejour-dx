using System.Numerics;

using Mokus2D.Integration.Farseer.Physics;

namespace ContreJourDX.Gameplay
{
    public interface IEatable : IBodyClip
    {
        void EatSpeedPauseScaleTime(Vector2 targetPosition, float finishSpeed, float pause, float scale, float time);

        float DeadEyeScale();

        bool CanDie();
    }
}
