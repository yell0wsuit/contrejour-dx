using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Mokus2D.Effects.Tweening.TweenToFrameData;

public struct NodeData
{
    public Vector2 Position;

    public float Rotation;

    public Vector2 Scale;

    public float Alpha;

    private Color Color;

    private float ColorRatio;

    private bool Visible;

    public NodeData(Node node)
    {
        this = default;
        Position = node.Position;
        Rotation = node.RotationDegrees;
        Scale = node.ScaleVec;
        Alpha = node.OpacityFloat;
        Color = node.Color;
        ColorRatio = node.ColorRatio;
        Visible = node.Visible;
    }
}
