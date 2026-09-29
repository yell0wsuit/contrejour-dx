using System;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using Mokus2D.Effects.Tweening;
using Mokus2D.Visual;

namespace Mokus2D.Util.Extensions
{
    public static class TweeningExtensions
    {
        public static TweenObject FadeOutAndRemove(this Node node, float time, Node target, int? tag = null)
        {
            return node.Tweener.Start(time, tag, target).Tween(NodeValues.OpacityFloat, 0f).OnComplete(NodeValues.RemoveFromParent);
        }

        public static TweenObject OnCompleteIfNotNull(this TweenObject tween, Action action)
        {
            if (action != null)
            {
                _ = tween.OnComplete(action);
            }
            return tween;
        }

        public static TweenObject FadeOutAndRemove(this Node node, float time, int? tag = null)
        {
            return node.FadeOutAndRemove(time, node, tag);
        }

        public static Sequence FadeOutAndHide(this Node node, float time, Node target)
        {
            return node.Tweener.StartSequence(time, target).Tween(NodeValues.OpacityFloat, 0f).OnComplete(NodeValues.Hide);
        }

        public static Sequence StartSequence(this Node node, float time)
        {
            return node.Tweener.StartSequence(time);
        }

        public static TweenObject SetAfter<T>(this Node node, float time, GetSetValue<T> getSet, T value)
        {
            return node.Tweener.Start(time).SetAfter(getSet, value);
        }

        public static TweenObject MoveTo(this Node node, float time, Vector2 position, Func<float, float> easing = null)
        {
            return node.Tweener.Start(time).MoveTo(position, easing);
        }

        public static T RotateTo<T>(this ITween<T> tween, float rotation, Func<float, float> easing = null) where T : ITween<T>
        {
            return tween.Tween(NodeValues.RotationRadians, rotation, easing);
        }

        public static T MoveTo<T>(this ITween<T> tween, Vector2 position, Func<float, float> easing = null) where T : ITween<T>
        {
            return tween.Tween(NodeValues.Position, position, easing);
        }

        public static TweenObject RotateTo(this Node node, float time, float angle, Func<float, float> easing = null)
        {
            return node.Tweener.Start(time).Tween(NodeValues.RotationRadians, angle, easing);
        }

        public static TweenObject ScaleTo(this Node node, float time, float scale, Func<float, float> easing = null)
        {
            return node.Tweener.Start(time).ScaleTo(scale, easing);
        }

        public static T ScaleTo<T>(this ITween<T> tweenObject, float scale, Func<float, float> easing = null) where T : ITween<T>
        {
            return tweenObject.Tween(NodeValues.Scale, scale, easing);
        }

        public static TweenObject ScaleTo(this Node node, float time, Vector2 scale, Func<float, float> easing = null)
        {
            return node.StartTween(NodeValues.ScaleVec, time, scale, easing);
        }

        private static TweenObject StartTween<T>(this Node node, GetSetValue<Node, T> getSet, float time, T value, Func<float, float> easing)
        {
            return node.Tweener.Start(time).Tween(getSet, value, easing);
        }

        public static Sequence TweenToColorThroughZero(this Node node, float time, Color color, int targetColorRatio = 1)
        {
            return node.ColorRatio == 0f
                ? node.Tweener.StartSequence(time).Tween(NodeValues.Color, color).Tween(NodeValues.ColorRatio, targetColorRatio)
                : node.TweenColorRatio(time / 2f, 0f).SetAfter(NodeValues.Color, color).Next(time / 2f)
                .Tween(NodeValues.ColorRatio, targetColorRatio);
        }

        public static Sequence TweenColorRatio(this Node node, float time, float colorRatio)
        {
            return node.Tweener.StartSequence(time).Tween(NodeValues.ColorRatio, colorRatio);
        }

        public static Sequence TweenColor(this Node node, float time, Color color, float colorRatio)
        {
            node.Color = color;
            return node.TweenColorRatio(time, colorRatio);
        }

        public static void FadeInAndOut(this Node node, float tweenTime, float visibleTime, float visibleOpacity = 1f)
        {
            node.OpacityFloat = 0f;
            node.Visible = true;
            _ = node.Tweener.StartSequence(tweenTime).Tween(NodeValues.OpacityFloat, visibleOpacity).Next(visibleTime)
                .Next(tweenTime)
                .Tween(NodeValues.OpacityFloat, 0f)
                .OnComplete(NodeValues.Hide);
        }

        public static TweenObject FadeTo(this Node node, float time, float value, int? tag = null)
        {
            return node.Tweener.Start(time, tag).Tween(NodeValues.OpacityFloat, value);
        }

        public static TweenObject CreateFadeTo(this Node node, float time, float value)
        {
            return node.Tweener.Create(time).Tween(NodeValues.OpacityFloat, value);
        }

        public static TweenObject FadeIn(this Node node, float time, int? tag = null)
        {
            return node.FadeTo(time, 1f, tag);
        }

        public static T FadeTo<T>(this ITween<T> tweenObject, float value, Func<float, float> easing = null) where T : ITween<T>
        {
            return tweenObject.Tween(NodeValues.OpacityFloat, value, easing);
        }

        public static T FadeIn<T>(this ITween<T> tweenObject, Func<float, float> easing = null) where T : ITween<T>
        {
            return tweenObject.FadeTo(1f, easing);
        }

        public static T FadeColorRatio<T>(this ITween<T> tweenObject, float colorRatio, Func<float, float> easing = null) where T : ITween<T>
        {
            return tweenObject.Tween(NodeValues.ColorRatio, colorRatio, easing);
        }

        public static T FadeOut<T>(this ITween<T> tweenObject, Func<float, float> easing = null) where T : ITween<T>
        {
            return tweenObject.FadeTo(0f, easing);
        }

        public static T AndHide<T>(this ITween<T> tweenObject) where T : ITween<T>
        {
            return tweenObject.OnComplete(NodeValues.Hide);
        }

        public static T AndRemove<T>(this ITween<T> tweenObject) where T : ITween<T>
        {
            return tweenObject.OnComplete(NodeValues.RemoveFromParent);
        }

        public static TweenObject CreateFadeIn(this Node node, float time)
        {
            return node.CreateFadeTo(time, 1f);
        }

        public static TweenObject FadeOut(this Node node, float time)
        {
            return node.FadeTo(time, 0f);
        }

        public static TweenObject FadeOutAndHide(this Node node, float time)
        {
            return node.FadeOut(time).OnComplete(NodeValues.Hide);
        }

        public static TweenObject ScheduleNext(this Node node, Action action)
        {
            return node.Schedule(0f, action);
        }

        public static TweenObject Schedule(this Node node, float time, Action action, int? tag = null)
        {
            return node.Tweener.Start(time, tag).OnComplete(action);
        }

        public static Task<bool> ScheduleAsync(this Node node, float time, int? tag = null)
        {
            TaskCompletionSource<bool> taskCompletition = new();
            _ = node.Tweener.Start(time, tag).OnComplete((Action<object>)delegate
            {
                taskCompletition.SetResult(result: true);
            });
            return taskCompletition.Task;
        }

        public static TweenObject Schedule(this Node node, float time, Action<object> action)
        {
            return node.Tweener.Start(time).OnComplete(action);
        }
    }
}
