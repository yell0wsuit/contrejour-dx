using System;

using Mokus2D.Util.Extensions;

namespace Mokus2D.Visual.Animation;

public static class AnimationExtensions
{
    public static int LastFrame(this IAnimatedNode node)
    {
        return (int)node.MaxFrame;
    }

    public static void SetProgress(this IAnimatedNode node, float progress)
    {
        node.CurrentFrame = (node.TotalFrames - 1) * progress;
    }

    public static void PlayAllOffsprings(this Node node)
    {
        node.SetOffspringsStoped(stoped: false);
    }

    public static void StopAllOffsprings(this Node node)
    {
        node.SetOffspringsStoped(stoped: true);
    }

    public static void SetOffspringsStoped(this Node node, bool stoped, Predicate<IAnimatedNode> predicate = null)
    {
        if (node is IAnimatedNode animatedNode && predicate.NullOrTrue(animatedNode))
        {
            animatedNode.Stoped = stoped;
        }
        foreach (Node child in node.Children)
        {
            child.SetOffspringsStoped(stoped, predicate);
        }
    }

    public static void SetOffspringsSpeed(this Node node, float speed, Predicate<IAnimatedNode> predicate = null)
    {
        if (node is IAnimatedNode animatedNode && predicate.NullOrTrue(animatedNode))
        {
            animatedNode.Speed = speed;
        }
        foreach (Node child in node.Children)
        {
            child.SetOffspringsSpeed(speed, predicate);
        }
    }
}
