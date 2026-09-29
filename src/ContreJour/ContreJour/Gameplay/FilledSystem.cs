using System;

using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Visual.Interfaces;

namespace ContreJour.Gameplay;

public class FilledSystem(IMovieClipData config) : ParticleSystem(config)
{
    public FilledSystem(string name)
        : this(Mokus2DGame.LoadResource<IMovieClipData>(name))
    {
    }

    public void Fill(Vector2 size)
    {
        int num = (int)Math.Ceiling(size.X / (TextureSize.X / 1f));
        int num2 = (int)Math.Ceiling(size.Y / (TextureSize.Y / 1f));
        for (int i = 0; i < num; i++)
        {
            for (int j = 0; j < num2; j++)
            {
                _ = AddParticle(new Vector2(i * TextureSize.X / 1f, j * TextureSize.Y / 1f));
            }
        }
    }
}
