using Microsoft.Xna.Framework;

namespace Mokus2D.Visual.Displacement;

public class MagneticNodeData
{
    public Vector2 DefaultPosition { get; }

    public Vector2 TargetPosition { get; set; }

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
