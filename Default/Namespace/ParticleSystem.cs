using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Visual;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Interfaces;

namespace Default.Namespace;

public class ParticleSystem : MultiframeSprite
{
    private readonly List<Particle> _particles = [];

    private readonly List<Particle> _invisibleParticles = [];

    private readonly List<Particle> _cachedInvisible = [];

    private bool _paused;

    public List<Particle> Particles => _particles;

    public bool Paused
    {
        get => _paused;
        set => _paused = value;
    }

    public ParticleSystem(string textureName)
        : this(Mokus2DGame.LoadSpriteData(textureName))
    {
    }

    public ParticleSystem(IMovieClipData data, int count)
        : base(data)
    {
        AddParticles(count);
    }

    public ParticleSystem(string textureName, int count)
        : this(Mokus2DGame.LoadSpriteData(textureName), count)
    {
    }

    public ParticleSystem(IMovieClipData data)
        : base(data)
    {
    }

    public ParticleSystem(ISpriteData config)
        : base(config)
    {
    }

    public ParticleSystem(ISpriteData config, int count)
        : this(config)
    {
        AddParticles(count);
    }

    public void AddParticles(int count)
    {
        for (int i = 0; i < count; i++)
        {
            _ = AddParticle(new Vector2(0f, 0f));
        }
    }

    public void SetParticleVisible(Particle particle)
    {
        if (!particle.Visible)
        {
            _particles.Add(particle);
            AddChild(particle);
            _ = _invisibleParticles.RemoveLast();
            particle.Visible = true;
            OnShowParticle(particle);
        }
    }

    protected virtual void OnShowParticle(Particle particle)
    {
    }

    public Particle AddOrGetInvisible()
    {
        if (_invisibleParticles.Count > 0)
        {
            Particle particle = _invisibleParticles.Last();
            SetParticleVisible(particle);
            return particle;
        }
        return AddParticle();
    }

    public Particle AddParticleWithFrame(int frame)
    {
        if (frame >= TotalFrames)
        {
            throw new ArgumentOutOfRangeException("frame");
        }
        Particle particle = CreateParticle();
        _particles.Add(particle);
        AddChild(particle);
        FrameData frameData = Frames[frame];
        particle.SetFrameData(frameData);
        particle.AnchorInPixels = TotalFrames > 1 ? (Anchor + frameData.Anchor) * ((IMovieClipData)Data).Size * Data.ScaleFactor : Anchor * Size;
        return particle;
    }

    public void ShowAllParticles()
    {
        while (_invisibleParticles.Count > 0)
        {
            SetParticleVisible(_invisibleParticles.Last());
        }
    }

    public void HideAllParticles()
    {
        foreach (Particle particle in _particles)
        {
            particle.Visible = false;
        }
    }

    public Particle AddParticle()
    {
        return AddParticleWithFrame(Maths.Random(TotalFrames));
    }

    public virtual Particle AddParticle(Vector2 position)
    {
        Particle particle = AddParticle();
        particle.Position = position;
        return particle;
    }

    public virtual Particle CreateParticle()
    {
        Particle particle = (Data is IMovieClipData) ? new Particle(this, (IMovieClipData)Data) : new Particle(this, (ISpriteData)Data);
        particle.Blend = Blend;
        return particle;
    }

    public override void Update(float time)
    {
        if (_paused)
        {
            return;
        }
        base.Update(time);
        _cachedInvisible.Clear();
        foreach (Particle particle in _particles)
        {
            UpdateParticleTime(particle, time);
            if (!particle.Visible)
            {
                _cachedInvisible.Add(particle);
            }
        }
        _invisibleParticles.AddItemsNoGarbage(_cachedInvisible);
        foreach (Particle item in _cachedInvisible)
        {
            _ = _particles.Remove(item);
            RemoveChild(item);
        }
    }

    public virtual void UpdateParticleTime(Particle particle, float time)
    {
    }

    protected override void BeginDraw(VisualState state)
    {
    }

    protected override void DrawSprite(VisualState state, Color color)
    {
    }

    public void RemoveParticle(Particle particle)
    {
        if (_particles.Contains(particle))
        {
            _ = _particles.Remove(particle);
            RemoveChild(particle);
        }
        if (_invisibleParticles.Contains(particle))
        {
            _ = _invisibleParticles.Remove(particle);
        }
    }
}
