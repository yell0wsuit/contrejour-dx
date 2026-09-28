using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Mokus2D.Visual.Displacement.Magnets;

public class GridMagnetGroup : GridMagnetBase
{
    private readonly List<GridMagnetBase> _magnets = [];

    public List<GridMagnetBase> Magnets => _magnets;

    public GridMagnetGroup(List<GridMagnetBase> magnets)
    {
        _magnets = magnets;
        CalculateBounds();
    }

    public GridMagnetGroup(params GridMagnetBase[] magnets)
    {
        _magnets = [];
        CalculateBounds();
    }

    private void CalculateBounds()
    {
        Rectangle rectangle = Rectangle.Empty;
        foreach (GridMagnetBase magnet in _magnets)
        {
            Rectangle bounds = magnet.Bounds;
            bounds.Offset((int)magnet.Position.X, (int)magnet.Position.Y);
            rectangle = Rectangle.Union(rectangle, bounds);
        }
        Bounds = rectangle;
    }

    public override Vector2 GetForce(Vector2 relativePosition)
    {
        Vector2 zero = Vector2.Zero;
        foreach (GridMagnetBase magnet in _magnets)
        {
            Vector2 relativePosition2 = relativePosition - magnet.Position;
            Vector2 force = magnet.GetForce(relativePosition2);
            zero += force;
        }
        return zero * Power;
    }
}
