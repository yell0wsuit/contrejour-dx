using System.Numerics;

using Mokus2D.Visual;

namespace Mokus2D.Effects.Tweening.TweenToFrameData
{
    public struct NodeData
    {
        public Vector2 Position;

        public float Rotation;

        public Vector2 Scale;

        public float Alpha;

        public NodeData(Node node)
        {
            this = default;
            Position = node.Position;
            Rotation = node.RotationDegrees;
            Scale = node.ScaleVec;
            Alpha = node.OpacityFloat;
        }
    }
}
