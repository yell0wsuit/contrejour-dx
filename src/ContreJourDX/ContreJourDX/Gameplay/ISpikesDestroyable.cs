using Mokus2D.Integration.Farseer.Physics;

namespace ContreJourDX.Gameplay
{
    public interface ISpikesDestroyable : IBodyClip
    {
        void Explode();

        void DoExplode();

        bool CanDie();
    }
}
