using ContreJour.Clips.menu;
using ContreJour.Config;

using Microsoft.Xna.Framework;

using Mokus2D.Effects.Tween.Easing;
using Mokus2D.Events;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Default.Namespace;

public class MovieStripesView : Node
{
    protected Node topSquare;

    protected Node bottomSquare;

    protected LayerColor FadeRectangle;

    public readonly EventSender RestartEvent = new();

    public readonly EventSender MenuEvent = new();

    protected float FinishDuration = 0.8f;

    public static readonly float StripesHeightIphone = 80f;

    private readonly bool _blackSide;

    public MovieStripesView(bool blackSide, bool fade)
    {
        _blackSide = blackSide;
        Vector2 rootSize = ContreJourConfig.RootSize;
        topSquare = new whitePixel
        {
            Color = Color.Black,
            ScaledSize = new Vector2(rootSize.X, StripesHeightIphone)
        };
        bottomSquare = new whitePixel
        {
            Color = Color.Black,
            ScaledSize = new Vector2(rootSize.X, StripesHeightIphone)
        };
        if (fade)
        {
            FadeRectangle = new LayerColor(Color.White, "menu/whitePixel");
            AddChild(FadeRectangle);
            FadeRectangle.OpacityFloat = 0f;
        }
        if (blackSide)
        {
            FadeRectangle.Color = ContreJourConstants.BlueLightColor;
        }
        AddChild(topSquare);
        AddChild(bottomSquare);
        bottomSquare.Position = new Vector2(0f, -4f);
        topSquare.Position = new Vector2(0f, rootSize.Y + StripesHeightIphone + 4f);
    }

    public void Show()
    {
        _ = (FadeRectangle?.FadeTo(FinishDuration, _blackSide ? 0.6f : 0.3f));
        _ = topSquare.MoveTo(FinishDuration, new Vector2(topSquare.Position.X, topSquare.Position.Y - StripesHeightIphone), Cubic.EaseIn);
        _ = bottomSquare.MoveTo(FinishDuration, new Vector2(bottomSquare.Position.X, bottomSquare.Position.Y + StripesHeightIphone), Cubic.EaseIn);
    }
}
