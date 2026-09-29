using Mokus2D.Interfaces;

namespace Mokus2D.Visual.Displacement.Magnets.Actions;

public abstract class MagnetAction(GridMagnetBase gridMagnet) : IUpdatable
{
    protected GridMagnetBase GridMagnet { get; set; } = gridMagnet;

    public abstract bool Finished { get; }

    public virtual void Update(float time)
    {
    }
}
