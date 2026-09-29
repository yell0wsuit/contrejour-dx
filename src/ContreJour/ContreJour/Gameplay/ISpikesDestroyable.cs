using Mokus2D.Integration.Farseer.Physics;

namespace ContreJour.Gameplay
{
    public interface ISpikesDestroyable : IBodyClip
    {
        void Explode();

        void DoExplode();

        bool CanDie();
    }
}
