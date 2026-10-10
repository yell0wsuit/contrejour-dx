using System.Collections.Generic;
using System.Numerics;

using ContreJourDX.Gameplay;

using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace ContreJourDX.Menu.SnapPanel
{
    public class SnapParticles : ParticleSystem
    {
        private static readonly Vector2[] path =
        [
            new(0f, 0f),
            new(-100f, 180f),
            new(140f, 230f),
            new(350f, 180f),
            new(500f, 70f)
        ];

        private readonly List<ButterFly> controllers = [];

        public SnapParticles(float particlesCount)
            : base("Win8PauseParticle")
        {
            float num = particlesCount / (path.Length - 1);
            for (int i = 0; i < path.Length - 1; i++)
            {
                Vector2 vector = path[i];
                Vector2 vector2 = path[i + 1];
                Vector2 vector3 = (vector2 - vector).Rotate90();
                vector3 = Vector2.Normalize(vector3);
                for (int j = 0; j < num; j++)
                {
                    _ = AddParticle(Vector2.Lerp(vector, vector2, j / num) + (vector3 * Maths.Random(-40f, 40f)));
                }
            }
        }

        public override Particle AddParticle(Vector2 position)
        {
            Particle particle = base.AddParticle(position);
            ButterFly item = new(particle, Maths.Random(0.8f, 1.5f));
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
}
