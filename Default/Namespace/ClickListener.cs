using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Events;
using Mokus2D.Input;

namespace Default.Namespace;

public class ClickListener : ITouchListener
{
    private readonly EventSender<Touch> clickEvent = new();

    private bool enabled;

    private bool listening;

    private float radius;

    private readonly Dictionary<Touch, Vector2> startPositions = [];

    public EventSender ClickEvent => clickEvent;

    public float Radius
    {
        get => radius;
        set => radius = value;
    }

    public virtual bool Enabled
    {
        get => enabled;
        set => enabled = value;
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
            _ = startPositions.Remove(touch);
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
            _ = startPositions.Remove(touch);
        }
    }

    protected virtual bool IsOutStartPosition(Touch touch, Vector2 startPosition)
    {
        return (touch.Position - startPosition).Length() > radius;
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
