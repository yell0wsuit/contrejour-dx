using System.Numerics;

using ContreJour.Gameplay;

using Xunit;

namespace ContreJour.Tests
{
    public class NewFriendBackgroundTests
    {
        [Theory]
        [InlineData(960f, 640f, 0f, 0f)]
        [InlineData(1920f, 1080f, 0f, 0f)]
        [InlineData(1024f, 768f, 0f, 0f)]
        [InlineData(640f, 960f, 0f, 0f)]
        [InlineData(2560f, 1080f, 0f, 0f)]
        [InlineData(1920f, 1080f, .5f, .5f)]
        public void BackgroundCoversViewportWithCenteredCrop(float width, float height, float anchorX, float anchorY)
        {
            Vector2 level = new(960f, 640f);
            Vector2 viewport = new(width, height);
            float gameScale = ContreJourGame.GetLevelScale(level, viewport, true);
            Vector2 gamePosition = new((width - (level.X * gameScale)) / 2f, height - (level.Y * gameScale));
            Vector2 boundsOrigin = -gamePosition / gameScale;
            Vector2 boundsSize = viewport / gameScale;
            Vector2 anchor = new(anchorX, anchorY);
            Vector2 imageSize = new(1280f, 720f);
            Vector2 authoredScale = new(1f / 1.3f, 1.1f / 1.3f);

            (Vector2 position, Vector2 scale) = ContreJourGame.FitNewFriendBackground(imageSize, anchor, authoredScale, boundsOrigin, boundsSize, 0f);
            Vector2 size = imageSize * scale;
            Vector2 lower = position + new Vector2(-anchor.X * size.X, (anchor.Y - 1f) * size.Y);
            Vector2 upper = position + new Vector2((1f - anchor.X) * size.X, anchor.Y * size.Y);
            Assert.True(lower.X <= boundsOrigin.X + .001f);
            Assert.True(lower.Y <= boundsOrigin.Y + .001f);
            Assert.True(upper.X >= boundsOrigin.X + boundsSize.X - .001f);
            Assert.True(upper.Y >= boundsOrigin.Y + boundsSize.Y - .001f);
            Assert.True(Vector2.Distance((lower + upper) / 2f, boundsOrigin + (boundsSize / 2f)) < .001f);
            Assert.Equal(authoredScale.X / authoredScale.Y, scale.X / scale.Y, 5);
        }

        [Fact]
        public void RotatedBackgroundCoversAllViewportCorners()
        {
            Vector2 imageSize = new(1280f, 720f);
            Vector2 viewport = new(1200f, 640f);
            Vector2 anchor = new(.2f, .7f);
            float rotation = .2f;
            (Vector2 position, Vector2 scale) = ContreJourGame.FitNewFriendBackground(imageSize, anchor, new Vector2(1f / 1.3f, 1.1f / 1.3f), Vector2.Zero, viewport, rotation);
            foreach (Vector2 corner in new[] { Vector2.Zero, new Vector2(viewport.X, 0f), viewport, new Vector2(0f, viewport.Y) })
            {
                Vector2 local = Vector2.Transform(corner - position, Matrix3x2.CreateRotation(-rotation));
                Assert.InRange(local.X, (-anchor.X * imageSize.X * scale.X) - .001f, ((1f - anchor.X) * imageSize.X * scale.X) + .001f);
                Assert.InRange(local.Y, ((anchor.Y - 1f) * imageSize.Y * scale.Y) - .001f, (anchor.Y * imageSize.Y * scale.Y) + .001f);
            }
        }
    }
}
