using Microsoft.Xna.Framework;

using Mokus2D.Visual.Interfaces;

namespace Default.Namespace;

public class GravityParticle : Particle
{
    protected Vector2 speed;

    protected float angularSpeed;

    public Vector2 Speed
    {
        get => speed;
        set => speed = value;
    }

    public float AngularSpeed
    {
        get => angularSpeed;
        set => angularSpeed = value;
    }

    public GravityParticle(ParticleSystem system, IMovieClipData data)
        : base(system, data)
    {
    }

    public GravityParticle(ParticleSystem system, ISpriteData data)
        : base(system, data)
    {
    }
}
