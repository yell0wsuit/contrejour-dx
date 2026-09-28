using Mokus2D.Interfaces;

namespace Default.Namespace;

public interface IGrassController : IUpdatable
{
    float Y { get; }

    void ScareFlyes(float offset);

    void OnTouchWith(float offset, BodyClip objectP);
}
