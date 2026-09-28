using ContreJour.Config;
using ContreJour.Utils;

using Microsoft.Xna.Framework;

using Mokus2D.Events;
using Mokus2D.Visual.Interactive;
using Mokus2D.Visual.Text;

namespace Default.Namespace;

public class ExitConfirmationView : ClickableLayer
{
    private Button _noButton;

    private Button _yesButton;

    private Label _label;

    private ColorRectangle _background;

    public EventSender<ExitConfirmationView> OnYes = new EventSender<ExitConfirmationView>();

    public EventSender<ExitConfirmationView> OnNo = new EventSender<ExitConfirmationView>();

    private void OnYesButtonClick(TouchArguments touchArguments)
    {
        OnYes.SendEvent(this);
    }

    private void OnNoButtonClick(TouchArguments touchArguments)
    {
        OnNo.SendEvent(this);
    }

    private void InitLayout()
    {
        float num = ContreJourConfig.RootSize.X * 0.5f;
        _yesButton = new Button("menu/McButtonMenuBackground", "menu/McButtonYes", "menu/McButtonYes")
        {
            Position = new Vector2(num - 80f, 280f)
        };
        _yesButton.StopEventPropagation = true;
        _noButton = new Button("menu/McButtonMenuBackground", "menu/McButtonNo", "menu/McButtonNo")
        {
            Position = new Vector2(num + 80f, 280f)
        };
        _noButton.StopEventPropagation = true;
        _yesButton.TouchEndEvent += OnYesButtonClick;
        _noButton.TouchEndEvent += OnNoButtonClick;
        _label = ContreJourLabelUtil.CreateLabel(28f, "EXIT_CONFIRM".Localize());
        _background = new ColorRectangle(new Color(0, 0, 0, 225), 2f * ContreJourConfig.RootSize);
        _label.Position = new Vector2(num, 380f);
        AddChild(_background);
        AddChild(_yesButton);
        AddChild(_noButton);
        AddChild(_label);
    }

    public ExitConfirmationView()
    {
        InitLayout();
    }
}
