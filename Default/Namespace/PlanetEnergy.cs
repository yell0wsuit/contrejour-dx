using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Mokus2D.Interfaces;
using Mokus2D.Visual.Particles.Util;

namespace Default.Namespace;

public class PlanetEnergy : IUpdatable
{
    private readonly List<Satellite> parts = new List<Satellite>();

    public PlanetEnergy(ParticleSystem system, Vector2 position, RandomRange? angleStep = null)
    {
        for (int i = 0; i < 5; i++)
        {
            Satellite satellite = new Satellite(null, system.AddParticle(position), null, (float)Math.PI * 2f / 5f * (float)i, position);
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
