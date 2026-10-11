using Mokus2D.Events;

namespace ContreJourDX.Gameplay
{
    public interface ISnotLinked
    {
        EventSender DestroyEvent { get; }

        int SnotJoinedCount { get; set; }

        bool SnotEnabled { get; }
    }
}
