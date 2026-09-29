using System;
using System.Numerics;

using Mokus2D.Data;
using Mokus2D.Effects.Tweening;
using Mokus2D.Graphics;
using Mokus2D.Util.Extensions;

namespace Mokus2D.Visual
{
    public static class NodeValues
    {
        public static readonly Action<object> Dispose = delegate (object n)
        {
            ((IDisposable)n).Dispose();
        };

        public static readonly Action<object> RemoveFromParent = delegate (object n)
        {
            ((Node)n).RemoveFromParent();
        };

        public static readonly Action<object> PlayFromStart = PlayFromStartAction;

        public static readonly Action<object> Play = PlayAction;

        public static readonly Action<object> RemoveFromParentAndFree = delegate (object n)
        {
            ((Node)n).RemoveFromParent();
            ((IFreeable)n).Free();
        };

        public static readonly Action<object> Free = delegate (object n)
        {
            ((IFreeable)n).Free();
        };

        public static readonly Action<object> RemoveFromParentFreeRemoveListeners = delegate (object n)
        {
            ((Node)n).RemoveFromParent();
            ((IFreeable)n).Free();
            ((AnimationNode)n).RemoveListeners();
        };

        public static readonly Action<object> Hide = delegate (object n)
        {
            ((Node)n).Visible = false;
        };

        public static readonly Action<object> Show = delegate (object n)
        {
            ((Node)n).Visible = true;
        };

        public static readonly Action<object> HideAndDisableUpdate = delegate (object n)
        {
            ((Node)n).VisibleAndUpdating = false;
        };

        public static readonly GetSetValue<Node, float> Scale = new(n => n.ScaleX, delegate (Node n, float v)
        {
            n.Scale = v;
        });

        public static readonly GetSetValue<ISizeNode, Vector2> ScaledSize = new(n => n.ScaledSize(), delegate (ISizeNode n, Vector2 v)
        {
            n.SetScaledSize(v);
        });

        public static readonly GetSetValue<Node, float> RotationRadians = new(n => n.RotationRadians, delegate (Node n, float v)
        {
            n.RotationRadians = v;
        });

        public static readonly GetSetValue<Node, float> ScaleY = new(n => n.ScaleY, delegate (Node n, float v)
        {
            n.ScaleY = v;
        });

        public static readonly GetSetValue<Node, float> ScaleX = new(n => n.ScaleX, delegate (Node n, float v)
        {
            n.ScaleX = v;
        });

        public static readonly GetSetValue<Node, Vector2> ScaleVec = new(n => n.ScaleVec, delegate (Node n, Vector2 v)
        {
            n.ScaleVec = v;
        });

        public static readonly GetSetValue<Node, float> OpacityFloat = new(n => n.OpacityFloat, delegate (Node n, float v)
        {
            n.OpacityFloat = v;
        });

        public static readonly GetSetValue<Node, Color> Color = new(n => n.Color, delegate (Node n, Color v)
        {
            n.Color = v;
        });

        public static readonly GetSetValue<Node, float> ColorRatio = new(n => n.ColorRatio, delegate (Node n, float v)
        {
            n.ColorRatio = v;
        });

        public static readonly GetSetValue<Node, Vector2> Position = new(n => n.Position, delegate (Node n, Vector2 v)
        {
            n.Position = v;
        });

        public static readonly GetSetValue<Node, float> X = new(n => n.X, delegate (Node n, float v)
        {
            n.X = v;
        });

        public static readonly GetSetValue<Node, float> Y = new(n => n.Y, delegate (Node n, float v)
        {
            n.Y = v;
        });

        public static readonly GetSetValue<Node, bool> Visible = new(n => n.Visible, delegate (Node n, bool v)
        {
            n.Visible = v;
        });

        public static readonly GetSetValue<Node, bool> VisibleLater = new(n => n.Visible, delegate (Node n, bool v)
        {
            n.CallLater(v ? Show : Hide);
        });

        public static readonly GetSetValue<Node, bool> VisibleAndUpdating = new(n => n.Visible, delegate (Node n, bool v)
        {
            n.VisibleAndUpdating = v;
        });

        public static readonly GetSetValue<Node, bool> UpdateChildren = new(n => n.UpdateChildren, delegate (Node n, bool v)
        {
            n.UpdateChildren = v;
        });

        public static readonly GetSetValue<Node, bool> IsAnimationDiscrete = new(n => n is AnimationNode animation && animation.IsChildrenAnimationsDiscrete, IsAnimationDiscreteSetter);

        public static readonly GetSetValue<Node, bool> Repeat = new(n => n is IAnimatedNode animated && animated.Repeat, RepeatSetter);

        public static readonly GetSetValue<Node, bool> Rewind = new(n => n is IAnimatedNode animated && animated.Rewind, RewindSetter);

        public static readonly GetSetValue<Node, bool> Stoped = new(n => n is IAnimatedNode animated && animated.Stoped, StopedSetter);

        public static readonly GetSetValue<Node, float> Speed = new(n => ((IAnimatedNode)n).Speed, delegate (Node n, float v)
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
}
