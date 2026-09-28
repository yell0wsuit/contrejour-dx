using System;

using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Visual.Interfaces;

namespace Default.Namespace;

public class FilledSystem : ParticleSystem
{
    public FilledSystem(string name)
        : this(Mokus2DGame.LoadResource<IMovieClipData>(name))
    {
    }

    public FilledSystem(IMovieClipData config)
        : base(config)
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
                AddParticle(new Vector2((float)i * TextureSize.X / 1f, (float)j * TextureSize.Y / 1f));
            }
        }
    }
}
