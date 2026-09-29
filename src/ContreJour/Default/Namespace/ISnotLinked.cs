using Mokus2D.Events;

namespace Default.Namespace;

public interface ISnotLinked
{
    EventSender DestroyEvent { get; }

    int SnotJoinedCount { get; set; }

    bool SnotEnabled { get; }
}
