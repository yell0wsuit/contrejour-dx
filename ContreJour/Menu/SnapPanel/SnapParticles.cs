using System.Collections.Generic;
using Default.Namespace;
using Microsoft.Xna.Framework;
using Mokus2D.Util.Extensions;

namespace ContreJour.Menu.SnapPanel;

public class SnapParticles : ParticleSystem
{
    private const int OrtoOffset = 40;

    private static readonly Vector2[] path = new Vector2[5]
    {
        new Vector2(0f, 0f),
        new Vector2(-100f, 180f),
        new Vector2(140f, 230f),
        new Vector2(350f, 180f),
        new Vector2(500f, 70f)
    };

    private readonly List<ButterFly> controllers = new List<ButterFly>();

    public SnapParticles(float particlesCount)
        : base("Win8PauseParticle")
    {
        float num = particlesCount / (float)(path.Length - 1);
        for (int i = 0; i < path.Length - 1; i++)
        {
            Vector2 vector = path[i];
            Vector2 vector2 = path[i + 1];
            Vector2 vector3 = (vector2 - vector).Rotate90();
            vector3.Normalize();
            for (int j = 0; (float)j < num; j++)
            {
                AddParticle(Vector2.Lerp(vector, vector2, (float)j / num) + vector3 * Maths.Random(-40f, 40f));
            }
        }
    }

    public override Particle AddParticle(Vector2 position)
    {
        Particle particle = base.AddParticle(position);
        ButterFly item = new ButterFly(particle, Maths.Random(0.8f, 1.5f));
        controllers.Add(item);
        return particle;
    }

    public override void Update(float time)
    {
        base.Update(time);
        foreach (ButterFly controller in controllers)
        {
            controller.Update(time);
        }
    }
}
