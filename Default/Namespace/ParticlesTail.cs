using Mokus2D.Interfaces;

namespace Default.Namespace;

public class ParticlesTail(BodyClip _clip) : IUpdatable
{
    protected BodyClip clip = _clip;

    public void Update(float time)
    {
    }
}
