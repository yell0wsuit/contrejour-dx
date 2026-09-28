using System;

using Mokus2D.Input;
using Mokus2D.Interfaces;
using Mokus2D.Util.MathUtils;

namespace Mokus2D.Controls;

public class GesturePager : ITouchListener, IDisposable, IUpdatable
{
    public int? MaxPosition;

    public float MinMoveOffset = 20f;

    public float MinMoveStep = 0.025f;

    public int? MinPosition;

    private float currentPosition;

    private Touch currentTouch;

    private float direction;

    private bool enabled = true;

    private float pageWidth;

    private int targetPosition;

    private float touchStartPosition;

    public bool Enabled
    {
        get
        {
            return enabled;
        }
        set
        {
            enabled = value;
            if (!value)
            {
                currentTouch = null;
            }
        }
    }

    public float CurrentPosition
    {
        get
        {
            return currentPosition;
        }
        set
        {
            currentPosition = value;
        }
    }

    public float PageWidth
    {
        get
        {
            return pageWidth;
        }
        set
        {
            pageWidth = value;
        }
    }

    public GesturePager()
    {
        Mokus2DGame.Instance.TouchController.AddListener(this);
        pageWidth = Mokus2DGame.Instance.ScreenSize.X;
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
        touchStartPosition = currentPosition;
        return true;
    }

    public bool TouchMove(Touch touch)
    {
        if (!Enabled)
        {
            return false;
        }
        currentPosition = touchStartPosition - touch.TotalOffset.X / pageWidth;
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
            targetPosition = (int)Math.Round(currentPosition);
        }
        else
        {
            SetTargetPosition();
        }
    }

    public void Update(float time)
    {
        if (currentTouch == null && currentPosition != (float)targetPosition)
        {
            float step = Math.Max((currentPosition - (float)targetPosition).Abs() / 10f, MinMoveStep) * time * 60f;
            currentPosition = currentPosition.StepTo(targetPosition, step);
        }
    }

    public void SetTargetPosition(int value)
    {
        targetPosition = value;
    }

    private void SetTargetPosition()
    {
        if (direction < 0f)
        {
            targetPosition = (int)Math.Floor(currentPosition);
        }
        else
        {
            targetPosition = (int)Math.Ceiling(currentPosition);
        }
        if (MinPosition.HasValue)
        {
            targetPosition = Math.Max(targetPosition, MinPosition.Value);
        }
        if (MaxPosition.HasValue)
        {
            targetPosition = Math.Min(targetPosition, MaxPosition.Value);
        }
    }
}
