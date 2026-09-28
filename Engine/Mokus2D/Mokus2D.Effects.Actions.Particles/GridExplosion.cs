using System;

using Microsoft.Xna.Framework;

using Mokus2D.Data;
using Mokus2D.Effects.Tweening;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Mokus2D.Effects.Actions.Particles;

public class GridExplosion : GridAction
{
    private static readonly Pool<GridExplosion> pool = new(() => new GridExplosion());

    private float minRadius;

    private float maxRadius;

    private Vector2 center;

    private float maxGridOffset;

    private float seconds;

    public static GridExplosion New(float seconds, float minRadius, float maxRadius)
    {
        return pool.New().Initialize(seconds, minRadius, maxRadius);
    }

    protected GridExplosion Initialize(float seconds, float minRadius, float maxRadius)
    {
        _ = Initialize();
        this.seconds = seconds;
        this.minRadius = minRadius;
        this.maxRadius = maxRadius;
        return this;
    }

    internal override void Start(float time)
    {
        center = Grid.GridSize / 2f;
        maxGridOffset = center.Length();
        base.Start(time);
    }

    protected override ITween CreateParticleUpdater(Node particle, int x, int y)
    {
        Vector2 vector = new Vector2(x, y) - center;
        if (vector != Vector2.Zero)
        {
            float length = MathHelper.Lerp(minRadius, maxRadius, 1f - (vector.Length() / maxGridOffset));
            _ = vector.Normalize(length);
        }
        else
        {
            _ = new Vector2(maxRadius, 0f);
        }
        throw new NotImplementedException();
    }
}
