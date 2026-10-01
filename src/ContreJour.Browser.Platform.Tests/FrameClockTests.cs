using Xunit;

namespace ContreJour.Browser.Platform.Tests
{
    public class FrameClockTests
    {
        [Fact]
        public void FirstStepIsZero()
        {
            FrameClock clock = new();

            Assert.Equal(0f, clock.Advance(5000));
        }

        [Fact]
        public void StepIsTheSecondsSinceThePreviousFrame()
        {
            FrameClock clock = new();
            _ = clock.Advance(1000);

            Assert.Equal(0.016f, clock.Advance(1016), 5);
        }

        // A frame run by a wake is stamped with performance.now(); the next animation frame carries its begin
        // time, which can be earlier. The game must never be stepped backward, nor be owed that time twice.
        [Fact]
        public void AnEarlierTimestampIsAZeroStepAndCountsFromTheLater()
        {
            FrameClock clock = new();
            _ = clock.Advance(100);

            Assert.Equal(0f, clock.Advance(95));
            Assert.Equal(0.0116f, clock.Advance(111.6), 5);
        }

        [Fact]
        public void ResetMakesTheNextStepZero()
        {
            FrameClock clock = new();
            _ = clock.Advance(100);

            clock.Reset();

            Assert.Equal(0f, clock.Advance(60_000));
            Assert.Equal(0.016f, clock.Advance(60_016), 5);
        }
    }
}
