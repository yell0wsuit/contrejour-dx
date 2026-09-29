using System;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;

namespace Default.Namespace;

public class DustData : IUpdatable
{
    private readonly float alphaDiff;
    private readonly ContreJourGame game;
    private readonly Particle particle;

    private Vector2 speed;

    protected ref Vector2 Speed => ref speed;

    public bool Dragging { get; set; }

    public bool ShouldRemove { get; private set; }

    public DustData(ContreJourGame game, Vector2 bodySpeed, Vector2 position, float speed, float alphaMult)
    {
        this.game = game;
        alphaDiff = Maths.Random(0.6f, 1.2f) / 255f;
        particle = this.game.Dust.AddOrGetInvisible();
        particle.Visible = true;
        position.Y -= 10f;
        particle.Position = position;
        particle.Scale = Maths.Random(0.5f, 1.5f);
        particle.OpacityFloat = Maths.Random(0.1f * alphaMult, 0.2f * alphaMult);
        this.Speed = Box2DConfig.DefaultConfig.ToPoint(bodySpeed);
        this.Speed.X *= Maths.Random(0.2f, 0.4f);
        this.Speed.Y = Math.Min(Maths.Random(5f, 20f) * speed, 40f);
    }

    public void Update(float time)
    {
        float num = time * 3f * Math.Min(particle.OpacityByte / 255f, 0.3f);
        Vector2 vector = Speed * num;
        particle.Position += vector;
        particle.OpacityFloat -= Dragging ? (alphaDiff * 10f) : alphaDiff;
        if (particle.OpacityByte <= 0)
        {
            particle.Visible = false;
            ShouldRemove = true;
        }
    }
}
