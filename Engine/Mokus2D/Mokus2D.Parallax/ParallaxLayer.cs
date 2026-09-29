using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Mokus2D.Parallax;

public class ParallaxLayer
{
    public Node Node { get; }

    public float Parallax { get; }

    public float ParallaxDistance { get; }

    public Vector2 InitialPosition { get; }

    public Vector2 InitialScale { get; set; }

    public ParallaxLayer(Node node, float parallax)
    {
        Node = node;
        Parallax = parallax;
        InitialPosition = node.Position;
        InitialScale = node.ScaleVec;
        ParallaxDistance = 1f / Parallax;
    }

    public ParallaxLayer(Node node, float parallax, Vector2 initialPosition)
    {
        Node = node;
        Parallax = parallax;
        InitialPosition = initialPosition;
        InitialScale = node.ScaleVec;
        ParallaxDistance = 1f / Parallax;
    }
}
