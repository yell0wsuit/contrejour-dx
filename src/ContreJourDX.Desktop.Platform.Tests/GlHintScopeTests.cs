using System;
using System.Collections.Generic;

using ContreJourDX.Desktop.Platform.Graphics;

using SDL3;

using Xunit;

namespace ContreJourDX.Desktop.Platform.Tests
{
    // What a renderer attempt leaves behind in SDL's process-wide hints.
    public class GlHintScopeTests
    {
        private const string Name = "CJ_TEST_HINT";

        [Fact]
        public void AHintWithNoPreviousValueIsUnsetAgain()
        {
            _ = SDL.ResetHint(Name);

            using (GlHintScope scope = new())
            {
                scope.Set(Name, "0");
                Assert.Equal("0", SDL.GetHint(Name));
            }

            Assert.Null(SDL.GetHint(Name));
        }

        [Fact]
        public void AHintThatAlreadyHadAValueGetsItBack()
        {
            _ = SDL.SetHint(Name, "original");

            using (GlHintScope scope = new())
            {
                scope.Set(Name, "0");
            }

            Assert.Equal("original", SDL.GetHint(Name));
            _ = SDL.ResetHint(Name);
        }

        [Fact]
        public void RestoringTwiceChangesNothingTheSecondTime()
        {
            _ = SDL.ResetHint(Name);
            GlHintScope scope = new();
            scope.Set(Name, "0");

            scope.Dispose();
            _ = SDL.SetHint(Name, "set by someone else");
            scope.Dispose();

            Assert.Equal("set by someone else", SDL.GetHint(Name));
            _ = SDL.ResetHint(Name);
        }

        [Fact]
        public void AHintSdlRefusesFailsTheAttempt()
        {
            // SDL refuses a normal-priority hint when the player set the matching environment
            // variable; the candidate must fail rather than run on a setting it does not have.
            GlHintScope scope = new(_ => null, (_, _) => false, _ => true);

            InvalidOperationException failure = Assert.Throws<InvalidOperationException>(() => scope.Set(SDL.Hints.FramebufferAcceleration, "0"));

            Assert.Contains(SDL.Hints.FramebufferAcceleration, failure.Message, StringComparison.Ordinal);
        }

        [Fact]
        public void ARefusedHintIsNotRestoredBecauseItNeverChanged()
        {
            List<string> reset = [];
            GlHintScope scope = new(_ => "from the environment", (_, _) => false, name =>
            {
                reset.Add(name);
                return true;
            });

            _ = Assert.Throws<InvalidOperationException>(() => scope.Set(SDL.Hints.FramebufferAcceleration, "0"));
            scope.Dispose();

            Assert.Empty(reset);
        }
    }
}
