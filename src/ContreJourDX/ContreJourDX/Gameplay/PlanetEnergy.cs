using System;
using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Interfaces;
using Mokus2D.Visual.Particles.Util;

namespace ContreJourDX.Gameplay
{
    public class PlanetEnergy : IUpdatable
    {
        private readonly List<Satellite> parts = [];

        public PlanetEnergy(ParticleSystem system, Vector2 position, RandomRange? angleStep = null)
        {
            for (int i = 0; i < 5; i++)
            {
                Satellite satellite = new(null, system.AddParticle(position), null, (float)Math.PI * 2f / 5f * i, position);
                parts.Add(satellite);
                if (angleStep.HasValue)
                {
                    satellite.AngleStep = angleStep.Value.GetValueInRange();
                }
            }
        }

        public void Update(float time)
        {
            foreach (Satellite part in parts)
            {
                part.Update(time);
            }
        }
    }
}
