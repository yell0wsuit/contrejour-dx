using System;

using Microsoft.Xna.Framework;

using Mokus2D.Data;
using Mokus2D.Effects.Tweening;
using Mokus2D.Util.Extensions;

namespace Mokus2D.Visual;

public static class NodeValues
{
    public static Action<object> Dispose = delegate (object n)
    {
        ((IDisposable)n).Dispose();
    };

    public static Action<object> RemoveFromParent = delegate (object n)
    {
        ((Node)n).RemoveFromParent();
    };

    public static Action<object> PlayFromStart = PlayFromStartAction;

    public static Action<object> Play = PlayAction;

    public static Action<object> RemoveFromParentAndFree = delegate (object n)
    {
        ((Node)n).RemoveFromParent();
        ((IFreeable)n).Free();
    };

    public static Action<object> Free = delegate (object n)
    {
        ((IFreeable)n).Free();
    };

    public static Action<object> RemoveFromParentFreeRemoveListeners = delegate (object n)
    {
        ((Node)n).RemoveFromParent();
        ((IFreeable)n).Free();
        ((AnimationNode)n).RemoveListeners();
    };

    public static Action<object> Hide = delegate (object n)
    {
        ((Node)n).Visible = false;
    };

    public static Action<object> Show = delegate (object n)
    {
        ((Node)n).Visible = true;
    };

    public static Action<object> HideAndDisableUpdate = delegate (object n)
    {
        ((Node)n).VisibleAndUpdating = false;
    };

    public static GetSetValue<Node, float> Scale = new GetSetValue<Node, float>((Node n) => n.ScaleX, delegate (Node n, float v)
    {
        n.Scale = v;
    });

    public static GetSetValue<ISizeNode, Vector2> ScaledSize = new GetSetValue<ISizeNode, Vector2>((ISizeNode n) => n.ScaledSize(), delegate (ISizeNode n, Vector2 v)
    {
        n.SetScaledSize(v);
    });

    public static GetSetValue<Node, float> RotationRadians = new GetSetValue<Node, float>((Node n) => n.RotationRadians, delegate (Node n, float v)
    {
        n.RotationRadians = v;
    });

    public static GetSetValue<Node, float> ScaleY = new GetSetValue<Node, float>((Node n) => n.ScaleY, delegate (Node n, float v)
    {
        n.ScaleY = v;
    });

    public static GetSetValue<Node, float> ScaleX = new GetSetValue<Node, float>((Node n) => n.ScaleX, delegate (Node n, float v)
    {
        n.ScaleX = v;
    });

    public static GetSetValue<Node, Vector2> ScaleVec = new GetSetValue<Node, Vector2>((Node n) => n.ScaleVec, delegate (Node n, Vector2 v)
    {
        n.ScaleVec = v;
    });

    public static GetSetValue<Node, float> OpacityFloat = new GetSetValue<Node, float>((Node n) => n.OpacityFloat, delegate (Node n, float v)
    {
        n.OpacityFloat = v;
    });

    public static GetSetValue<Node, Color> Color = new GetSetValue<Node, Color>((Node n) => n.Color, delegate (Node n, Color v)
    {
        n.Color = v;
    });

    public static GetSetValue<Node, float> ColorRatio = new GetSetValue<Node, float>((Node n) => n.ColorRatio, delegate (Node n, float v)
    {
        n.ColorRatio = v;
    });

    public static GetSetValue<Node, Vector2> Position = new GetSetValue<Node, Vector2>((Node n) => n.Position, delegate (Node n, Vector2 v)
    {
        n.Position = v;
    });

    public static GetSetValue<Node, float> X = new GetSetValue<Node, float>((Node n) => n.X, delegate (Node n, float v)
    {
        n.X = v;
    });

    public static GetSetValue<Node, float> Y = new GetSetValue<Node, float>((Node n) => n.Y, delegate (Node n, float v)
    {
        n.Y = v;
    });

    public static GetSetValue<Node, bool> Visible = new GetSetValue<Node, bool>((Node n) => n.Visible, delegate (Node n, bool v)
    {
        n.Visible = v;
    });

    public static GetSetValue<Node, bool> VisibleLater = new GetSetValue<Node, bool>((Node n) => n.Visible, delegate (Node n, bool v)
    {
        n.CallLater(v ? Show : Hide);
    });

    public static GetSetValue<Node, bool> VisibleAndUpdating = new GetSetValue<Node, bool>((Node n) => n.Visible, delegate (Node n, bool v)
    {
        n.VisibleAndUpdating = v;
    });

    public static GetSetValue<Node, bool> UpdateChildren = new GetSetValue<Node, bool>((Node n) => n.UpdateChildren, delegate (Node n, bool v)
    {
        n.UpdateChildren = v;
    });

    public static GetSetValue<Node, bool> IsAnimationDiscrete = new GetSetValue<Node, bool>((Node n) => n is AnimationNode && ((AnimationNode)n).IsChildrenAnimationsDiscrete, IsAnimationDiscreteSetter);

    public static GetSetValue<Node, bool> Repeat = new GetSetValue<Node, bool>((Node n) => n is IAnimatedNode && ((IAnimatedNode)n).Repeat, RepeatSetter);

    public static GetSetValue<Node, bool> Rewind = new GetSetValue<Node, bool>((Node n) => n is IAnimatedNode && ((IAnimatedNode)n).Rewind, RewindSetter);

    public static GetSetValue<Node, bool> Stoped = new GetSetValue<Node, bool>((Node n) => n is IAnimatedNode && ((IAnimatedNode)n).Stoped, StopedSetter);

    public static GetSetValue<Node, float> Speed = new GetSetValue<Node, float>((Node n) => ((IAnimatedNode)n).Speed, delegate (Node n, float v)
    {
        ((IAnimatedNode)n).Speed = v;
    });

    private static void PlayFromStartAction(object node)
    {
        if (node is IAnimatedNode animatedNode)
        {
            animatedNode.GotoAndPlay(0f);
        }
    }

    private static void PlayAction(object node)
    {
        if (node is IAnimatedNode animatedNode)
        {
            animatedNode.Stoped = false;
        }
    }

    private static void RepeatSetter(Node node, bool value)
    {
        if (node is IAnimatedNode animatedNode)
        {
            animatedNode.Repeat = value;
        }
    }

    private static void RewindSetter(Node node, bool value)
    {
        if (node is IAnimatedNode animatedNode)
        {
            animatedNode.Rewind = value;
        }
    }

    private static void StopedSetter(Node node, bool value)
    {
        if (node is IAnimatedNode animatedNode)
        {
            animatedNode.Stoped = value;
        }
    }

    private static void IsAnimationDiscreteSetter(Node node, bool value)
    {
        if (node is AnimationNode animationNode)
        {
            animationNode.IsChildrenAnimationsDiscrete = value;
        }
    }
}
