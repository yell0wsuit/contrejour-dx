using System;
using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Data;

namespace Mokus2D.Visual.Animation
{
    internal sealed class SmoothAnimationPlayer : IAnimationNodePlayer
    {
        public void ApplyFrameData(AnimationNode node, float frame)
        {
            bool flag = !node.TweenEdgeFrames || (!node.Repeat && (int)frame == (int)node.MaxFrame) || (int)frame == (int)node.MinFrame;
            int num = (int)(node.Rewind ? Math.Ceiling(frame) : Math.Floor(frame));
            int num2 = node.Rewind ? (num - 1) : (num + 1);
            if (flag && num > node.MaxFrame)
            {
                num = (int)node.MaxFrame;
            }
            num = Maths.ModPositive(num, node.TotalFrames);
            num2 = (int)Maths.ModPositive(num2, (float)node.TotalFrames).Clamp(node.MinFrame, node.MaxFrame);
            if (flag && ((!node.Rewind && num2 < num) || (node.Rewind && num2 > num)))
            {
                num2 = num;
            }
            List<AnimationFrameData> list = node.AnimationData[num];
            List<AnimationFrameData> list2 = node.AnimationData[num2];
            float num3 = frame % 1f;
            if (node.Rewind)
            {
                num3 = 1f - num3;
            }
            for (int i = 0; i < list.Count; i++)
            {
                AnimationFrameData animationFrameData = list[i];
                if (node.AnimatedChildren.ContainsKey(animationFrameData.Id))
                {
                    AnimationFrameData nextData = list2[i];
                    Node child = node.GetChild(animationFrameData.Id);
                    float offset = child.IsAnimationDiscrete ? 0f : num3;
                    ApplyChildFrameData(node, child, animationFrameData, nextData, offset);
                }
            }
        }

        private static void ApplyChildFrameData(AnimationNode node, Node child, AnimationFrameData previousData, AnimationFrameData nextData, float offset)
        {
            if (!child.IgnoredAnimations.Visible)
            {
                child.Visible = previousData.Visible;
            }
            Vector2 vector = (node.Root != null) ? node.Root.SpritesScaleFactor.Signs() : Vector2.One;
            if (!child.IgnoredAnimations.Position)
            {
                child.Position = XnaMath.Lerp(previousData.Position, nextData.Position, offset) * vector;
            }
            if (!child.IgnoredAnimations.Rotation)
            {
                float value = Maths.SimplifyAngleDegrees(nextData.Rotation, previousData.Rotation - 180f);
                child.RotationDegrees = XnaMath.Lerp(previousData.Rotation, value, offset) * vector.X * vector.Y;
            }
            if (!child.IgnoredAnimations.Opacity)
            {
                child.OpacityFloat = XnaMath.Lerp(previousData.Alpha, nextData.Alpha, offset);
            }
            if (!child.IgnoredAnimations.Scale)
            {
                child.ScaleVec = XnaMath.Lerp(previousData.Scale, nextData.Scale, offset);
            }
            if (!child.IgnoredAnimations.Color)
            {
                child.Color = Color.Lerp(previousData.Color, nextData.Color, offset);
                child.ColorRatio = offset.Lerp(previousData.ColorRatio, nextData.ColorRatio);
            }
        }
    }
}
