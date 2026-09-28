using Mokus2D.Events;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;

namespace Default.Namespace;

public abstract class TouchEffect
{
    public readonly EventSender ChangeEvent = new();

    protected Node Node;

    protected float effectTime;

    protected bool isOn;

    public float EffectTime
    {
        get => effectTime;
        set => effectTime = value;
    }

    public bool IsOn
    {
        get => isOn;
        set
        {
            if (value != isOn)
            {
                isOn = value;
                if (value)
                {
                    OnAction(Node);
                }
                else
                {
                    OffAction(Node);
                }
                ChangeEvent.SendEvent();
            }
        }
    }

    public TouchEffect(TouchSprite sprite)
        : this((Node)sprite)
    {
        sprite.TouchBeginEvent += OnTouchBegan;
        sprite.TouchEndEvent += OnTouchEnd;
        sprite.TouchOutEvent += OnTouchEnd;
    }

    public TouchEffect(Node node)
    {
        Node = node;
        effectTime = 0.1f;
    }

    public abstract void OnAction(Node node);

    public abstract void OffAction(Node node);

    public void OnTouchBegan(TouchArguments touchArguments)
    {
        IsOn = true;
    }

    public void OnTouchEnd(TouchArguments touchArguments)
    {
        IsOn = false;
    }
}
