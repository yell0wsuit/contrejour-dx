using System;
using Microsoft.Xna.Framework;
using Mokus2D.Interfaces;

namespace Default.Namespace;

public class DustData : IUpdatable
{
    protected float alphaDiff;

    protected bool dragging;

    protected ContreJourGame game;

    protected bool hasRemove;

    protected Particle particle;

    protected Vector2 speed;

    public bool Dragging
    {
        get
        {
            return dragging;
        }
        set
        {
            dragging = value;
        }
    }

    public bool ShouldRemove => hasRemove;

    public DustData(ContreJourGame _game, Vector2 bodySpeed, Vector2 position, float _speed, float alphaMult)
    {
        game = _game;
        alphaDiff = Maths.Random(0.6f, 1.2f) / 255f;
        particle = game.Dust.AddOrGetInvisible();
        particle.Visible = true;
        position.Y -= 10f;
        particle.Position = position;
        particle.Scale = Maths.Random(0.5f, 1.5f);
        particle.OpacityFloat = Maths.Random(0.1f * alphaMult, 0.2f * alphaMult);
        speed = Box2DConfig.DefaultConfig.ToPoint(bodySpeed);
        speed.X *= Maths.Random(0.2f, 0.4f);
        speed.Y = Math.Min(Maths.Random(5f, 20f) * _speed, 40f);
    }

    public void Update(float time)
    {
        float num = time * 3f * Math.Min((float)particle.OpacityByte / 255f, 0.3f);
        Vector2 vector = speed * num;
        particle.Position += vector;
        particle.OpacityFloat -= (dragging ? (alphaDiff * 10f) : alphaDiff);
        if (particle.OpacityByte <= 0)
        {
            particle.Visible = false;
            hasRemove = true;
        }
    }
}
