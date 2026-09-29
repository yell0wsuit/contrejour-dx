using Mokus2D.Events;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;

namespace ContreJour.Gameplay;

public abstract class TouchEffect(Node node)
{
    private readonly EventSender ChangeEvent = new();

    protected Node Node { get; set; } = node;

    public float EffectTime { get; set; } = 0.1f;

    public bool IsOn
    {
        get;
        set
        {
            if (value != field)
            {
                field = value;
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
