using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Visual.Particles.Util;

namespace Default.Namespace;

public class PlanetSurround : IUpdatable
{
    private const int PARTS_COUNT = 128;

    private readonly List<ButterFly> parts = new List<ButterFly>(128);

    private RandomRange orbit = new RandomRange(115f, 5f);

    private RandomRange startScale = new RandomRange(0.3f, 0.15f);

    public PlanetSurround(ParticleSystem system)
    {
        system.Position = new Vector2(system.Position.X + 6f, system.Position.Y - 6f);
        for (int i = 0; i < 128; i++)
        {
            float num = Maths.Random((float)Math.PI * -2f, (float)Math.PI * 2f);
            Vector2 position = new Vector2(orbit.GetValueInRange() * (float)Math.Cos(num), orbit.GetValueInRange() * (float)Math.Sin(num));
            ButterFly item = new ButterFly(_scale: startScale.GetValueInRange(), _particle: system.AddParticle(position));
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
