using System;

using Microsoft.Xna.Framework;

using Mokus2D.Data;
using Mokus2D.Effects.Tweening;
using Mokus2D.Interfaces;
using Mokus2D.Visual;
using Mokus2D.Visual.Particles.Grid;

namespace Mokus2D.Effects.Actions.Particles;

public abstract class GridAction : ITween, ICleanable, IUpdatable
{
    protected GridSystem Grid => throw new NotImplementedException();

    public bool Finished { get; private set; }

    protected GridAction Initialize()
    {
        return this;
    }

    protected void SetTargets()
    {
    }

    internal virtual void Start(float time)
    {
        Vector2 gridSize = Grid.GridSize;
        for (int i = 0; i < gridSize.Y; i++)
        {
            for (int j = 0; j < gridSize.X; j++)
            {
                Node particle = Grid.GetParticle(j, i);
                CreateAction(time, particle, j, i);
            }
        }
    }

    protected void CreateAction(float time, Node particle, int x, int y)
    {
        _ = CreateParticleUpdater(particle, x, y);
    }

    protected abstract ITween CreateParticleUpdater(Node particle, int x, int y);

    public void Clean()
    {
        throw new NotImplementedException();
    }

    public void Update(float time)
    {
        throw new NotImplementedException();
    }

    public void Free()
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public ITween OnComplete(Action action)
    {
        throw new NotImplementedException();
    }
}
