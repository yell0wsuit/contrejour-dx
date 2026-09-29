using Microsoft.Xna.Framework;

using Mokus2D.Visual.Interfaces;

namespace ContreJour.Gameplay;

public class GravityParticle : Particle
{
    private Vector2 speed;

    public Vector2 Speed
    {
        get => speed;
        set => speed = value;
    }

    public float AngularSpeed { get; set; }

    public GravityParticle(ParticleSystem system, IMovieClipData data)
        : base(system, data)
    {
    }

    public GravityParticle(ParticleSystem system, ISpriteData data)
        : base(system, data)
    {
    }
}
