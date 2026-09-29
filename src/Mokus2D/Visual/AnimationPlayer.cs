using System;

using Mokus2D.Interfaces;
using Mokus2D.Util;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace Mokus2D.Visual
{
    public class AnimationPlayer : IUpdatable
    {
        private float maxFrame;
        private readonly float FPS = Mokus2DGame.Config.AnimationFPS;

        private readonly IAnimatedNode _owner;

        public bool SyncToParent { get; set; }

        public bool Rewind { get; set; }

        public bool Repeat { get; set; }

        public float Speed { get; set; }

        public bool Stoped { get; set; }

        public float MinFrame
        {
            get;
            set
            {
                value = value.Clamp(0f, _owner.TotalFrames - 1);
                if (field != value)
                {
                    field = value;
                    maxFrame = Math.Max(maxFrame, field + 1f);
                    if (CurrentFrame < field)
                    {
                        CurrentFrame = field;
                    }
                }
            }
        }

        public float MaxFrame
        {
            get => maxFrame;
            set
            {
                value = value.Clamp(1f, _owner.TotalFrames);
                if (maxFrame != value)
                {
                    maxFrame = value - 0.0001f;
                    if (CurrentFrame > maxFrame)
                    {
                        CurrentFrame = maxFrame;
                    }
                }
            }
        }

        public int FrameValue => (int)CurrentFrame;

        public float CurrentFrame
        {
            get;
            set
            {
                if (field != value)
                {
                    if ((value > maxFrame || value < MinFrame) && !Repeat)
                    {
                        value = value.Clamp(MinFrame, maxFrame);
                        Stoped = true;
                    }
                    while (value > maxFrame)
                    {
                        value = value - maxFrame + MinFrame;
                    }
                    while (value < MinFrame)
                    {
                        value = value - MinFrame + maxFrame;
                    }
                    field = value;
                }
            }
        }

        public event Action EndEvent;

        public AnimationPlayer(IAnimatedNode owner)
        {
            _owner = owner;
            Speed = 1f;
            Repeat = true;
            MaxFrame = owner.TotalFrames;
            SyncToParent = ((Node)owner).Config?.GetBool("syncToParent") ?? false;
        }

        public void Update(float time)
        {
            if (SyncToParent && ((Node)_owner).Parent is IAnimatedNode animatedNode)
            {
                CurrentFrame = animatedNode.CurrentFrame;
            }
            else if (!Stoped)
            {
                int num = (!Rewind) ? 1 : (-1);
                float num2 = time * FPS * Speed * num;
                float num3 = CurrentFrame += num2;
                if ((num3 > maxFrame && !Rewind) || (num3 < MinFrame && Rewind))
                {
                    EndEvent.Dispatch();
                }
            }
        }

        public void GotoAndPlay(float frame)
        {
            Stoped = false;
            CurrentFrame = frame;
        }

        public void GotoAndStop(float frame)
        {
            Stoped = true;
            CurrentFrame = frame;
        }
    }
}
