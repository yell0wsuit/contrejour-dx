using Microsoft.Xna.Framework;

using Mokus2D.Events;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;

namespace Default.Namespace.Windows;

public class PopUpWindow : Node
{
    public readonly EventSender OpenChangeEvent = new EventSender();

    private FadeEffect howerEffect;

    private bool open;

    protected readonly Node container = new Node();

    protected readonly ClickableLayer clickableLayer = new ClickableLayer();

    public bool Open
    {
        get
        {
            return open;
        }
        set
        {
            if (open != value)
            {
                open = value;
                Tweener.Stop();
                if (open)
                {
                    OnOpen();
                    Visible = true;
                    howerEffect.IsOn = true;
                    this.Schedule(howerEffect.EffectTime, ShowItems);
                    container.Visible = false;
                    UpdateEnabled = true;
                }
                else
                {
                    howerEffect.IsOn = true;
                    this.Schedule(howerEffect.EffectTime, HideItems);
                    clickableLayer.InteractionsEnabled = false;
                }
                OpenChangeEvent.SendEvent();
            }
        }
    }

    public PopUpWindow()
    {
        AddChild(container);
        clickableLayer.InteractionsEnabled = false;
        container.AddChild(clickableLayer, 1);
        LayerColor layerColor = new LayerColor(Color.Black, "menu/whitePixel");
        AddChild(layerColor);
        howerEffect = new FadeEffect(layerColor);
        layerColor.Visible = false;
        layerColor.OpacityByte = 0;
        container.Visible = false;
        UpdateEnabled = false;
    }

    protected virtual void OnOpen()
    {
    }

    private void HideItems()
    {
        this.Schedule(howerEffect.EffectTime, Disable);
        howerEffect.IsOn = false;
        container.Visible = false;
    }

    private void Disable()
    {
        UpdateEnabled = (Visible = false);
    }

    private void ShowItems()
    {
        howerEffect.IsOn = false;
        container.Visible = true;
        clickableLayer.InteractionsEnabled = true;
    }
}
