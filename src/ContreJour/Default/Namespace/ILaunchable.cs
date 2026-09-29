using Mokus2D.Events;

namespace Default.Namespace;

public interface ILaunchable : IRadius, IBodyClip
{
    EventSender DestroyEvent { get; }

    bool HitEnabled { set; }

    void SetSpeedLocked(bool value);

    bool CanLaunch();
}
