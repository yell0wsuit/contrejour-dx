using ContreJour.Clips.common;

using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class RoundDragBodyClip : DragableBodyClip
{
    protected float radius;

    protected Sprite middleSprite;

    private static readonly Vector2 TOUCH_CENTER_OFFSET = new(42f, 42f);

    public override Vector2 PositionVec => base.PositionVec + TouchOffset();

    public override Vector2 SnotPosition => Body.Position;

    public RoundDragBodyClip(ContreJourLevelBuilder _builder, object _body, Node _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
    }

    protected override string ReplaceClipName(ContreJourLevelBuilder _builder)
    {
        return _builder.ContreJour.ChooseSide(null, "McRoundDragViewWhite", "McRoundDragView_5", null);
    }

    protected override void CreateBoundsClip(float scale)
    {
        middleSprite = new McRoundDragFrameView();
        radius = 200f * scale * builder.EngineConfig.SizeMultiplier / 2f;
        middleSprite.Scale = scale * 200f / 200f;
        middleSprite.Position = clip.Position;
        builder.Add(middleSprite, -1);
    }

    protected override void RefreshObjectsAlpha()
    {
        middleSprite.OpacityByte = (int)currentAlpha;
    }

    protected override Vector2 TouchOffset()
    {
        return builder.ToVec(TOUCH_CENTER_OFFSET);
    }

    protected override Vector2 GetDragPosition(Vector2 offset)
    {
        return VectorUtil.ClampLength(ref offset, radius) + initialPosition;
    }
}
