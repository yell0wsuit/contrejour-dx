using System;
using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Interfaces;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Particles.Util;

namespace ContreJour.Gameplay
{
    public class PlanetSurround : IUpdatable
    {
        private readonly List<ButterFly> parts = new(128);

        private RandomRange orbit = new(115f, 5f);

        private RandomRange startScale = new(0.3f, 0.15f);

        public PlanetSurround(ParticleSystem system)
        {
            system.Position = new Vector2(system.Position.X + 6f, system.Position.Y - 6f);
            for (int i = 0; i < 128; i++)
            {
                float num = Maths.Random((float)Math.PI * -2f, (float)Math.PI * 2f);
                Vector2 position = new(orbit.GetValueInRange() * (float)Math.Cos(num), orbit.GetValueInRange() * (float)Math.Sin(num));
                ButterFly item = new(scale: startScale.GetValueInRange(), particle: system.AddParticle(position));
                parts.Add(item);
            }
        }

        public void Update(float time)
        {
            foreach (ButterFly part in parts)
            {
                part.Update(time);
            }
        }
    }
}
