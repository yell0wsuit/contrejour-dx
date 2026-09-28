using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Data;

namespace Mokus2D.UI.Layout.Anchors;

public class AnchorPositionCalculator
{
    private readonly Dictionary<string, AnimationData> _originalAnimationData = new Dictionary<string, AnimationData>();

    public readonly Vector2 DevelopmentSize;

    public Vector2 CurrentSize;

    public AnchorPositionCalculator(Vector2 developmentSize)
    {
        DevelopmentSize = developmentSize;
    }

    public AnchorPositionCalculator(Vector2 developmentSize, Vector2 currentSize)
    {
        DevelopmentSize = developmentSize;
        CurrentSize = currentSize;
    }

    public void ApplyPosition(Node node, Vector2 anchor)
    {
        Pair<AnimationFrameData> frameData = GetFrameData(node);
        ApplyPosition(node, frameData.First.Position, anchor);
        frameData.Second.Position = node.Position;
    }

    public void ApplyPosition(Node node, Vector2 position, Vector2 anchor)
    {
        node.Position = GetPosition(position, anchor);
    }

    public void Stretch(ISizeNode sizeNode)
    {
        ApplyPositionAndSize(sizeNode, Vector2.Zero, Vector2.One);
    }

    public void ApplyPositionAndSize(ISizeNode sizeNode, Vector2 leftTopAnchor, Vector2 rightBottomAnchor)
    {
        Node node = (Node)sizeNode;
        Vector2 developmentPosition = node.Position + sizeNode.Size * node.ScaleVec;
        ApplyPosition(node, leftTopAnchor);
        Vector2 position = GetPosition(developmentPosition, rightBottomAnchor);
        node.ScaleVec = (position - node.Position) / sizeNode.Size;
    }

    public Vector2 GetPosition(Vector2 developmentPosition, Vector2 anchor)
    {
        Vector2 vector = CurrentSize - DevelopmentSize;
        return developmentPosition + anchor * vector;
    }

    public void AdjustHeight(Node node, float bottomPosition)
    {
        Pair<AnimationFrameData> frameData = GetFrameData(node);
        float num = CurrentSize.Y - (DevelopmentSize.Y - bottomPosition);
        node.ScaleY = frameData.First.Scale.Y * num / bottomPosition;
        frameData.Second.Scale.Y = node.ScaleY;
    }

    public void AdjustHeight(Node node)
    {
        AdjustHeight(node, DevelopmentSize.Y);
    }

    public void AdjustWidth(Node node)
    {
        AdjustWidth(node, DevelopmentSize.X);
    }

    public void AdjustWidth(Node node, float rightPosition)
    {
        Pair<AnimationFrameData> frameData = GetFrameData(node);
        float num = CurrentSize.X - (DevelopmentSize.X - rightPosition);
        node.ScaleX = frameData.First.Scale.X * num / rightPosition;
        frameData.Second.Scale.X = node.ScaleX;
    }

    private Pair<AnimationFrameData> GetFrameData(Node node)
    {
        AnimationNode animationNode = (AnimationNode)node.Parent;
        string id = ((IId)animationNode).Id;
        AnimationData animationData = _originalAnimationData.TryGetValue(id);
        if (animationData == null)
        {
            animationData = animationNode.AnimationData.DeepClone();
            _originalAnimationData[id] = animationData;
        }
        AnimationFrameData childFrameData = animationData.GetChildFrameData(0, node.Name);
        AnimationFrameData childFrameData2 = animationNode.AnimationData.GetChildFrameData(0, node.Name);
        return new Pair<AnimationFrameData>(childFrameData, childFrameData2);
    }
}
