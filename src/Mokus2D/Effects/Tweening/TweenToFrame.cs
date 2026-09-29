using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Data;
using Mokus2D.Effects.Tweening.TweenToFrameData;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;
using Mokus2D.Visual.Data;

namespace Mokus2D.Effects.Tweening
{
    public class TweenToFrame : IntervalTweenBase
    {
        private static readonly Pool<TweenToFrame> Pool = new(() => new TweenToFrame());

        private readonly Dictionary<Node, NodeTweenData> _data = [];

        private bool _animationWasEnabled;

        private List<AnimationFrameData> _targetFrame;

        private AnimationNode _target;

        private bool _finished;

        public static TweenToFrame New(AnimationNode target, List<AnimationFrameData> frame, float seconds)
        {
            return Pool.New().Initialize(target, frame, seconds);
        }

        private TweenToFrame Initialize(AnimationNode target, List<AnimationFrameData> frame, float seconds)
        {
            _finished = false;
            _target = target;
            _targetFrame = frame;
            Initialize(seconds);
            return this;
        }

        protected override void Start()
        {
            _animationWasEnabled = _target.AnimationEnabled;
            _target.AnimationEnabled = false;
            foreach (Node child in _target.Children)
            {
                bool flag = false;
                foreach (AnimationFrameData item in _targetFrame)
                {
                    if (item.Id == child.Name)
                    {
                        _data[child] = new NodeTweenData(child, item);
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    _data[child] = new NodeTweenData(child, null);
                }
            }
        }

        protected override void Finish()
        {
            _finished = true;
            _target.AnimationEnabled = _animationWasEnabled;
        }

        public override void Clean()
        {
            base.Clean();
            if (!_finished)
            {
                _target.AnimationEnabled = _animationWasEnabled;
            }
            _data.Clear();
            _target = null;
        }

        protected override void UpdateRatio(float ratio)
        {
            foreach (KeyValuePair<Node, NodeTweenData> datum in _data)
            {
                Node key = datum.Key;
                NodeTweenData value = datum.Value;
                if (datum.Value.TargetFrame != null)
                {
                    float value2 = Maths.SimplifyAngleDegrees(value.StartData.Rotation, value.TargetFrame.Rotation - 180f);
                    key.Position = Vector2.Lerp(value.StartData.Position, value.TargetFrame.Position, ratio);
                    key.RotationDegrees = MathHelper.Lerp(value2, value.TargetFrame.Rotation, ratio);
                    key.ScaleVec = Vector2.Lerp(value.StartData.Scale, value.TargetFrame.Scale, ratio);
                    key.OpacityFloat = MathHelper.Lerp(value.StartData.Alpha, value.TargetFrame.Alpha, ratio);
                }
                else
                {
                    key.OpacityFloat = MathHelper.Lerp(value.StartData.Alpha, 0f, ratio);
                }
            }
        }

        public override void Free()
        {
            Pool.Free(this);
        }
    }
}
