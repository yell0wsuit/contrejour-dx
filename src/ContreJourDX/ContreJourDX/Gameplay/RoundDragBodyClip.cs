using System.Numerics;

using ContreJourDX.Clips;

using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class RoundDragBodyClip(ContreJourDXLevelBuilder builder, object body, Node clip, Hashtable config) : DragableBodyClip(builder, body, clip, config)
    {
        private float radius;

        private Sprite middleSprite;

        private static readonly Vector2 TouchCenterOffset = new(42f, 42f);

        public override Vector2 PositionVec => base.PositionVec + TouchOffset();

        public override Vector2 SnotPosition => Body.Position;

        protected override string ReplaceClipName(ContreJourDXLevelBuilder builder)
        {
            return builder.ContreJourDX.NewFriendChapter ? "McRoundDragView_7"
                : builder.ContreJourDX.ChooseSide(null, "McRoundDragViewWhite", "McRoundDragView_5", null);
        }

        protected override void CreateBoundsClip(float scale)
        {
            middleSprite = new Sprite(((ContreJourDXGame)Builder.Game).NewFriendChapter
                ? "newFriend/McRoundDragFrameView_7" : ClipIds.Common.McRoundDragFrameView);
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
}
