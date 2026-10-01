using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Util.Data;
using Mokus2D.Visual.Data;

using Xunit;

namespace Mokus2D.Tests
{
    public class AnimationDataCloneTests
    {
        private static AnimationData Sample()
        {
            AnimationData data = new()
            {
                PrecalculatedBounds = new Rectangle(1, 2, 3, 4),
                Config = new Dictionary<string, string> { ["a"] = "1" },
            };
            data.AddInstanceConfig("child", new Dictionary<string, string> { ["b"] = "2" });
            AnimationFrameData frame = new()
            {
                Id = "child",
                Position = new Vector2(5f, 6f),
                Rotation = 7f,
                Alpha = 0.5f,
                Color = new Color(10, 20, 30),
                ColorRatio = 0.25f,
                Visible = false,
                Scale = new Vector2(2f, 3f)
            };
            data.Add([frame]);
            return data;
        }

        [Fact]
        public void CopiesEveryValue()
        {
            AnimationData clone = Sample().Clone();

            Assert.Equal(new Rectangle(1, 2, 3, 4), clone.PrecalculatedBounds);
            Assert.Equal("1", clone.Config["a"]);
            Assert.Equal("2", clone.GetInstanceConfig("child")["b"]);
            AnimationFrameData frame = clone.GetChildFrameData(0, "child");
            Assert.Equal(new Vector2(5f, 6f), frame.Position);
            Assert.Equal(new Vector2(2f, 3f), frame.Scale);
            Assert.Equal(7f, frame.Rotation);
            Assert.Equal(0.5f, frame.Alpha);
            Assert.Equal(new Color(10, 20, 30), frame.Color);
            Assert.Equal(0.25f, frame.ColorRatio);
            Assert.False(frame.Visible);
        }

        // The anchor layout keeps the clone as the untouched original while it rewrites the live frames.
        [Fact]
        public void ChangingTheOriginalLeavesTheCloneAlone()
        {
            AnimationData original = Sample();
            AnimationData clone = original.Clone();

            original.GetChildFrameData(0, "child").Scale.X = 9f;
            original.GetChildFrameData(0, "child").Position = Vector2.Zero;
            original.Config["a"] = "changed";
            original.GetInstanceConfig("child")["b"] = "changed";
            original.Add([]);

            Assert.Equal(2f, clone.GetChildFrameData(0, "child").Scale.X);
            Assert.Equal(new Vector2(5f, 6f), clone.GetChildFrameData(0, "child").Position);
            Assert.Equal("1", clone.Config["a"]);
            Assert.Equal("2", clone.GetInstanceConfig("child")["b"]);
            Assert.Equal(1, clone.Count);
        }
    }
}
