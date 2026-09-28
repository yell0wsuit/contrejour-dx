using ContreJour.Config;

using Mokus2D.Events;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;

namespace Default.Namespace;

public class BackMovieStripes : MovieStripesView
{
    private Button backButton;

    public readonly EventSender BackEvent = new();

    private ClickableLayer clickableLayer;

    public BackMovieStripes()
        : base(blackSide: false, fade: false)
    {
        backButton = new Button("menu/McBackIcon");
        _ = backButton.FadeIn(2f);
        backButton.OpacityByte = 0;
        _ = ScreenConstants.W7FromIPhoneSize;
        backButton.Position = ContreJourConfig.BackButtonPosition;
        backButton.RealScale = 1.3f;
        clickableLayer = new ClickableLayer();
        AddChild(clickableLayer, 5);
        clickableLayer.AddChild(backButton);
        backButton.TouchEndEvent += OnBackClick;
        backButton.Color = ContreJourConstants.GreyColor;
        backButton.Visible = false;
        ShowBack();
    }

    public void ShowBack()
    {
        backButton.Visible = true;
        _ = backButton.FadeIn(2f);
    }

    private void OnBackClick(TouchArguments touchArguments)
    {
        BackEvent.SendEvent();
        BackEvent.Enabled = false;
    }
}
