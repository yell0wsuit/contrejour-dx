using ContreJour.Clips.chapter5;

using Microsoft.Xna.Framework;

using Mokus2D.Effects.Tween.Easing;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Default.Namespace;

public class FlyWings : Node
{
    protected Sprite bottom;

    protected Sprite top;

    protected Node topContainer;

    protected Node bottomContainer;

    protected bool flying;

    private static readonly float ROTATION_DIFF = 5.ToRadians();

    private static readonly float ROTATION = 25.ToRadians();

    private static readonly float IDLE_ROTATION = 10.ToRadians();

    public FlyWings()
    {
        top = new McFlyWing();
        bottom = new McFlyWing();
        topContainer = new Node();
        bottomContainer = new Node();
        topContainer.AddChild(top);
        bottomContainer.AddChild(bottom);
        top.ScaleY = 0.5f;
        bottom.ScaleY = 0.5f;
        AddChild(topContainer);
        AddChild(bottomContainer);
        topContainer.RotationRadians = 0f - IDLE_ROTATION;
        bottomContainer.RotationRadians = IDLE_ROTATION;
        StartActionDiff(top, 0f - ROTATION_DIFF);
        StartActionDiff(bottom, ROTATION_DIFF);
        ScaleY = 0.7f;
    }

    public void SetFlying(bool value)
    {
        if (flying != value)
        {
            flying = value;
            float num = flying ? ROTATION : IDLE_ROTATION;
            _ = topContainer.RotateTo(0.5f, num);
            _ = bottom.RotateTo(0.5f, 0f - num);
            float y = flying ? 1f : 0.5f;
            _ = top.ScaleTo(0.5f, new Vector2(1f, y));
            _ = bottom.ScaleTo(0.5f, new Vector2(1f, y));
            _ = this.ScaleTo(0.5f, new Vector2(y: flying ? 1f : 0.7f, x: ScaleX));
        }
    }

    public static void StartActionDiff(Sprite wing, float diff)
    {
        _ = wing.Tweener.RepeatSequenceForever(1.5f).Tween(NodeValues.RotationRadians, diff, Cubic.EaseInOut).Next(1.5f)
            .Tween(NodeValues.RotationRadians, 0f - diff, Cubic.EaseInOut);
    }
}
