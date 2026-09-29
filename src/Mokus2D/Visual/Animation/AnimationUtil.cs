using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Data;

namespace Mokus2D.Visual.Animation;

public static class AnimationUtil
{
    public static void ApplyChildTransformations(AnimationNode source, AnimationNode target, bool recursive = false)
    {
        foreach (string childrenName in target.ChildrenNames)
        {
            if (source.HasChild(childrenName))
            {
                Node child = target.GetChild(childrenName);
                Node child2 = source.GetChild(childrenName);
                ApplyChildTransformations(recursive, child, child2);
            }
        }
    }

    private static void ApplyChildTransformations(bool recursive, Node targetChild, Node sourceChild)
    {
        targetChild.ApplyTransformations(sourceChild);
        if (recursive && sourceChild is AnimationNode sourceAnimation && targetChild is AnimationNode targetAnimation)
        {
            ApplyChildTransformations(sourceAnimation, targetAnimation, recursive: true);
        }
    }

    public static void ApplyChildTransformationsAndVisibility(AnimationNode source, AnimationNode target, bool recursive = false)
    {
        foreach (string childrenName in target.ChildrenNames)
        {
            if (source.HasChild(childrenName))
            {
                Node child = target.GetChild(childrenName);
                Node child2 = source.GetChild(childrenName);
                ApplyChildTransformations(recursive, child, child2);
                child.Visible = child2.Visible;
            }
        }
    }

    public static void ApplyChildFrameData(AnimationNode node, Node child, AnimationFrameData data)
    {
        if (!child.IgnoredAnimations.Visible)
        {
            child.Visible = data.Visible;
        }
        Vector2 vector = (node.Root != null) ? node.Root.SpritesScaleFactor.Signs() : Vector2.One;
        if (!child.IgnoredAnimations.Position)
        {
            child.Position = data.Position * vector;
        }
        if (!child.IgnoredAnimations.Rotation)
        {
            child.RotationDegrees = data.Rotation * vector.X * vector.Y;
        }
        if (!child.IgnoredAnimations.Opacity)
        {
            child.OpacityFloat = data.Alpha;
        }
        if (!child.IgnoredAnimations.Scale)
        {
            child.ScaleVec = data.Scale;
        }
        if (!child.IgnoredAnimations.Color)
        {
            child.Color = data.Color;
            child.ColorRatio = data.ColorRatio;
        }
    }

    public static void ApplyAnimationConfig(IAnimatedNode animated)
    {
        Node node = (Node)animated;
        if (node.Config != null)
        {
            animated.Speed = node.Config.GetFloat("speed", 1f);
        }
    }
}
