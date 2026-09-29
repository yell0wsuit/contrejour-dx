using System.CodeDom.Compiler;

using ContreJour.Gameplay;

using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Data;
using Mokus2D.Input;
using Mokus2D.Interfaces;
using Mokus2D.Visual;
using Mokus2D.Visual.Text;

namespace ContreJour.Clips.menu2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class petitInformation : AnimationNode, ITouchListener, IFreeable, IId
{
    public enum State
    {
        FullscreenMessage,
        TouchMessage
    }

    public const string ID = "menu2/petitInformation";

    private State _currentState;

    public State CurrentState
    {
        get
        {
            return _currentState;
        }
        set
        {
            _currentState = value;
            RefreshState();
        }
    }

    public petitInformationBackground background { get; protected set; }

    public Label message { get; protected set; }

    public resizeIcon resize { get; protected set; }

    public touchIcon touch { get; protected set; }

    public string Id => "menu2/petitInformation";

    protected override void Initialize()
    {
        base.Initialize();
        Mokus2DGame.Instance.TouchController.AddListener(this);
    }

    private void RefreshState()
    {
        message.TextString = ((_currentState == State.FullscreenMessage) ? "FULLSCREEN_TO_PLAY".Localize() : "CREATED_FOR_TOUCH".Localize());
        touch.Visible = _currentState == State.TouchMessage;
        resize.Visible = _currentState == State.FullscreenMessage;
    }

    public bool TouchBegin(Touch touch)
    {
        if (Visible && CurrentState == State.FullscreenMessage)
        {
            TryEnterFullscreen();
        }
        return false;
    }

    public bool TouchMove(Touch touch)
    {
        return false;
    }

    public void TouchEnd(Touch touch)
    {
    }

    public void TryEnterFullscreen()
    {
        // Windows 8 "snapped" view has no desktop equivalent.
    }

    public static petitInformation New()
    {
        petitInformation petitInformation2 = StaticPool.New<petitInformation>();
        petitInformation2.RefreshProperties();
        return petitInformation2;
    }

    public petitInformation()
        : base("menu2/petitInformation")
    {
        background = new petitInformationBackground();
        AddChild("background", background);
        message = new Label("Segoe Print", 30f, new Vector2(457.55f, 118.2f))
        {
            TextString = "Open window in fullscreen \nto play\n",
            LineSpacing = 2f,
            Align = TextAlign.Center
        };
        AddChild("message", message);
        resize = new resizeIcon();
        AddChild("resize", resize);
        touch = new touchIcon();
        AddChild("touch", touch);
        Initialize();
    }

    public void Free()
    {
        StaticPool.Free<petitInformation>(this);
    }
}
