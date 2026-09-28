using Mokus2D.Interfaces;

namespace Default.Namespace;

public class ParticlesTail : IUpdatable
{
    protected BodyClip clip;

    public ParticlesTail(BodyClip _clip)
    {
        clip = _clip;
    }

    public void Update(float time)
    {
    }
}
