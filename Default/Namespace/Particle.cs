using Mokus2D.Visual;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Interfaces;

namespace Default.Namespace;

public class Particle : MultiframeSprite
{
    protected ParticleSystem System;

    public object Tag;

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
        base.TextureRectangle = frameData.Rect;
    }
}
