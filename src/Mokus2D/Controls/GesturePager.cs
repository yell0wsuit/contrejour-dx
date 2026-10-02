using System;

using Mokus2D.Input;
using Mokus2D.Interfaces;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace Mokus2D.Controls
{
    public sealed class GesturePager : ITouchListener, IDisposable, IUpdatable
    {
        public int? MaxPosition { get; set; }

        private readonly float MinMoveOffset = 20f;

        private readonly float MinMoveStep = 0.025f;

        public int? MinPosition { get; set; }
        private Touch currentTouch;

        private float direction;

        private float touchStartPosition;

        public bool Enabled
        {
            get;
            set
            {
                field = value;
                if (!value)
                {
                    currentTouch = null;
                }
            }
        } = true;

        public float CurrentPosition { get; set; }

        public int TargetPosition { get; private set; }

        public float PageWidth { get; set; }

        public GesturePager()
        {
            Mokus2DGame.Instance.TouchController.AddListener(this);
            PageWidth = Mokus2DGame.Instance.ScreenSize.X;
        }

        public void Dispose()
        {
            Mokus2DGame.Instance.TouchController.RemoveListener(this);
        }

        public bool TouchBegin(Touch touch)
        {
            if (currentTouch != null || !Enabled)
            {
                return false;
            }
            currentTouch = touch;
            touchStartPosition = CurrentPosition;
            return true;
        }

        public bool TouchMove(Touch touch)
        {
            if (!Enabled)
            {
                return false;
            }
            CurrentPosition = touchStartPosition - (touch.TotalOffset.X / PageWidth);
            if (touch.LastFrameOffset.X != 0f)
            {
                direction = 0f - touch.LastFrameOffset.X.Sign();
            }
            return true;
        }

        public void TouchEnd(Touch touch)
        {
            currentTouch = null;
            if (Math.Abs(touch.TotalOffset.X) < MinMoveOffset)
            {
                TargetPosition = (int)Math.Round(CurrentPosition);
            }
            else
            {
                SetTargetPosition();
            }
        }

        public void Update(float time)
        {
            if (currentTouch == null && CurrentPosition != TargetPosition)
            {
                float step = Math.Max((CurrentPosition - TargetPosition).Abs() / 10f, MinMoveStep) * time * 60f;
                CurrentPosition = CurrentPosition.StepTo(TargetPosition, step);
            }
        }

        public void SetTargetPosition(int value)
        {
            TargetPosition = value;
        }

        private void SetTargetPosition()
        {
            TargetPosition = direction < 0f ? (int)Math.Floor(CurrentPosition) : (int)Math.Ceiling(CurrentPosition);
            if (MinPosition.HasValue)
            {
                TargetPosition = Math.Max(TargetPosition, MinPosition.Value);
            }
            if (MaxPosition.HasValue)
            {
                TargetPosition = Math.Min(TargetPosition, MaxPosition.Value);
            }
        }
    }
}
