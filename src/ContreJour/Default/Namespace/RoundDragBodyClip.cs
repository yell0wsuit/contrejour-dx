using ContreJour.Clips.common;

using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class RoundDragBodyClip(ContreJourLevelBuilder builder, object body, Node clip, Hashtable config) : DragableBodyClip(builder, body, clip, config)
{
    private float radius;

    private McRoundDragFrameView middleSprite;

    private static readonly Vector2 TouchCenterOffset = new(42f, 42f);

    public override Vector2 PositionVec => base.PositionVec + TouchOffset();

    public override Vector2 SnotPosition => Body.Position;

    protected override string ReplaceClipName(ContreJourLevelBuilder builder)
    {
        return builder.ContreJour.ChooseSide(null, "McRoundDragViewWhite", "McRoundDragView_5", null);
    }

    protected override void CreateBoundsClip(float scale)
    {
        middleSprite = new McRoundDragFrameView();
        radius = 200f * scale * Builder.EngineConfig.SizeMultiplier / 2f;
        middleSprite.Scale = scale * 200f / 200f;
        middleSprite.Position = Clip.Position;
        Builder.Add(middleSprite, -1);
    }

    protected override void RefreshObjectsAlpha()
    {
        middleSprite.OpacityByte = (int)CurrentAlpha;
    }

    protected override Vector2 TouchOffset()
    {
        return Builder.ToVec(TouchCenterOffset);
    }

    protected override Vector2 GetDragPosition(Vector2 offset)
    {
        return VectorUtil.ClampLength(ref offset, radius) + InitialPosition;
    }
}
