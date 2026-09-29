using Mokus2D.Events;
using Mokus2D.Integration.Farseer.Physics;

namespace ContreJour.Gameplay
{
    public interface ILaunchable : IRadius, IBodyClip
    {
        EventSender DestroyEvent { get; }

        bool HitEnabled { set; }

        void SetSpeedLocked(bool value);

        bool CanLaunch();
    }
}
