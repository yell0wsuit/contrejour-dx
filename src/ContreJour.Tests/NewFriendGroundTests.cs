using ContreJour.Gameplay;

using Mokus2D.Graphics;

using Xunit;

namespace ContreJour.Tests
{
    public class NewFriendGroundTests
    {
        [Fact]
        public void FloorGradientClampsItsStopsAtTheCanvasEdgeBeforeInterpolating()
        {
            Assert.Equal(Color.Black, NewFriendGroundBorder.StrokeColor(0, 0f, 0f, 640f, 1f));
            Assert.Equal(new Color(110, 55, 17), NewFriendGroundBorder.StrokeColor(0, 19.2f, 0f, 640f, 1f));
            Assert.Equal(new Color(221, 110, 34), NewFriendGroundBorder.StrokeColor(0, 38.4f, 0f, 640f, 1f));
        }

        [Fact]
        public void GrassSitsAboveTheWebSurfaceByTheAuthoredShadingOffset()
        {
            Assert.Equal(1f / 12f, NewFriendGroundBorder.SurfaceOffset);
            Assert.Equal(19.8f, (NewFriendGroundBorder.GrassOffset - NewFriendGroundBorder.SurfaceOffset) * 30f, 4);
        }

        [Fact]
        public void GroundHasBlackFillAndOrangeExteriorShading()
        {
            Assert.Equal(Color.Black, PlasticineConstants.NewFriendLight.LightInColor);
            Color outer = NewFriendGroundBorder.StrokeColor(0, 450f, 400f, 640f, 1f);
            Color inner = NewFriendGroundBorder.StrokeColor(9, 450f, 400f, 640f, 1f);
            Assert.Equal(new Color(221, 110, 34), outer);
            Assert.Equal(new Color(2, 1, 0), inner);
            Assert.Equal(Color.Black, NewFriendGroundBorder.StrokeColor(0, 300f, 400f, 640f, 1f));
        }

        [Fact]
        public void ShadingDropOffUsesAuthoredHeightAndExtendsFloorToBottom()
        {
            Assert.Equal(425f, NewFriendGroundBorder.DropOff(450f, 300f));
            Assert.Equal(440f, NewFriendGroundBorder.DropOff(450f, 410f));
            Assert.Equal(0f, NewFriendGroundBorder.DropOff(80f, -40f));
        }
    }
}
