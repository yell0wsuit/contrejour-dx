using Mokus2D.Interfaces;

namespace Mokus2D.Visual.Displacement.Magnets.Actions;

public abstract class MagnetAction : IUpdatable
{
    protected GridMagnetBase GridMagnet;

    public abstract bool Finished { get; }

    protected MagnetAction(GridMagnetBase gridMagnet)
    {
        GridMagnet = gridMagnet;
    }

    public virtual void Update(float time)
    {
    }
}
