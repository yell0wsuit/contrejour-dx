using System;

using Microsoft.Xna.Framework;

using Mokus2D.Effects.Tweening;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace Mokus2D.Visual.Util;

public static class NodeUtil
{
    public static bool IsBranchVisible(Node node)
    {
        while (node != null && !node.IsRoot && node.Visible)
        {
            node = node.Parent;
        }
        return node?.IsRoot ?? false;
    }

    public static void MoveWithTransformations(this Node node, Node newParent)
    {
        if (node.Parent != newParent)
        {
            node.TransformToNode(newParent);
            node.RemoveFromParent();
            newParent.AddChild(node);
        }
    }

    public static void SetToAllOffsprings<T>(this Node node, GetSetValue<T> getSet, T value)
    {
        getSet.SetValue(node, value);
        foreach (Node child in node.Children)
        {
            child.SetToAllOffsprings(getSet, value);
        }
    }

    public static void SetToAllChildren<T>(this Node node, GetSetValue<T> getSet, T value)
    {
        foreach (Node child in node.Children)
        {
            getSet.SetValue(child, value);
        }
    }

    public static void ApplyToAllOffsprings(this Node node, Action<Node> action)
    {
        action(node);
        foreach (Node child in node.Children)
        {
            child.ApplyToAllOffsprings(action);
        }
    }

    public static void ApplyToAllChilren(this Node node, Action<Node> action)
    {
        foreach (Node child in node.Children)
        {
            action(child);
        }
    }

    public static void TransformToNode(this Node node, Node newParent)
    {
        Vector2 vector = node.ZeroToNode(newParent);
        Vector2 vector2 = node.LocalToNode(new Vector2(1f, 0f), newParent, refreshTransformations: false) - vector;
        float num = vector2.Atan2();
        Vector2 vector3 = node.LocalToNode(new Vector2(0f, 1f), newParent, refreshTransformations: false) - vector;
        node.Position = vector;
        node.RotationRadians = num;
        node.ScaleX = vector2.Length();
        node.ScaleY = vector3.Length();
        float angle = vector3.Atan2() - ((float)Math.PI / 2f);
        if (!Maths.FuzzyEquals(Maths.SimplifyAngle(angle), Maths.SimplifyAngle(num), 0.1f))
        {
            node.ScaleY *= -1f;
        }
    }
}
