using Microsoft.Xna.Framework;

namespace Mokus2D.Visual.Displacement;

public class MagneticNodeData
{
    public readonly Vector2 DefaultPosition;

    public Vector2 TargetPosition;

    public MagneticNodeData(Vector2 defaultPosition)
    {
        DefaultPosition = defaultPosition;
        Clean();
    }

    public void Clean()
    {
        TargetPosition = DefaultPosition;
    }
}
