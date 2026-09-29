using Mokus2D.Visual;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Interfaces;

namespace ContreJour.Gameplay;

public class Particle : MultiframeSprite
{
    private readonly ParticleSystem System;

    public object Tag { get; set; }

    public Particle(ParticleSystem system, ISpriteData data)
        : base(data)
    {
        System = system;
    }

    public Particle(ParticleSystem system, IMovieClipData data)
        : base(data)
    {
        System = system;
    }

    public override void RemoveFromParent()
    {
        System.RemoveParticle(this);
    }

    public void SetFrameData(FrameData frameData)
    {
        TextureRectangle = frameData.Rect;
    }
}
