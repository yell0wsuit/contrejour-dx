using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Mokus2D;
using Mokus2D.Events;
using Mokus2D.Input;

namespace Default.Namespace;

public class ClickListener : ITouchListener
{
    private const float DEFAULT_RADIUS = 20f;

    protected readonly EventSender<Touch> clickEvent = new EventSender<Touch>();

    protected bool enabled;

    protected bool listening;

    protected float radius;

    protected Dictionary<Touch, Vector2> startPositions = new Dictionary<Touch, Vector2>();

    public EventSender ClickEvent => clickEvent;

    public float Radius
    {
        get
        {
            return radius;
        }
        set
        {
            radius = value;
        }
    }

    public virtual bool Enabled
    {
        get
        {
            return enabled;
        }
        set
        {
            enabled = value;
        }
    }

    public ClickListener(int priority = 0)
    {
        Mokus2DGame.Instance.TouchController.AddListener(this, priority);
        listening = true;
        radius = 20f;
        enabled = true;
    }

    public virtual bool TouchBegin(Touch touch)
    {
        startPositions[touch] = touch.Position;
        return true;
    }

    public virtual bool TouchMove(Touch touch)
    {
        if (IsOutStartPosition(touch, startPositions[touch]))
        {
            startPositions.Remove(touch);
            return false;
        }
        return true;
    }

    public virtual void TouchEnd(Touch touch)
    {
        if (startPositions.ContainsKey(touch))
        {
            if (Enabled)
            {
                clickEvent.SendEvent(touch);
            }
            startPositions.Remove(touch);
        }
    }

    protected virtual bool IsOutStartPosition(Touch touch, Vector2 _startPosition)
    {
        return (touch.Position - _startPosition).Length() > radius;
    }

    public void Remove()
    {
        if (listening)
        {
            Mokus2DGame.Instance.TouchController.RemoveListener(this);
            listening = false;
        }
    }
}
