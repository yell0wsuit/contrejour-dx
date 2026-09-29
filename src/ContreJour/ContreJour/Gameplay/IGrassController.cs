using Mokus2D.Interfaces;

namespace ContreJour.Gameplay
{
    public interface IGrassController : IUpdatable
    {
        float Y { get; }

        void ScareFlyes(float offset);

        void OnTouchWith(float offset, BodyClip objectP);
    }
}
