using System;
using System.Linq;
using System.Numerics;

using ContreJour.Gameplay;
using ContreJour.Gameplay.Eyes;
using ContreJour.Menu.LevelComplete;

using Mokus2D;
using Mokus2D.Input;
using Mokus2D.PlatformSupport.Input;
using Mokus2D.Visual;

namespace ContreJour.Regression
{
    internal static class MangoInteractionRegression
    {
        public static void VerifyAssets(ContreJourGame game)
        {
            Check(game.BonusChapter && !game.NewFriendChapter, "Mango selected the wrong chapter theme.");
            Check(ReferenceEquals(game.Grass.Texture, Mokus2DGame.LoadMovieClipData("chapter6/McGrass_6").Texture),
                "Mango grass did not load from the green atlas.");
            foreach (SnotPoint point in game.SnotPoints)
            {
                Check(point.Clip is Sprite sprite && ReferenceEquals(sprite.Texture, Mokus2DGame.LoadSpriteData("chapter6/McSnotPoint").Texture),
                    "A movable tentacle anchor is missing its Mango artwork.");
            }
            foreach (SpikesFlowerBodyClip flower in game.Builder.World.BodyList.Select(body => body.UserData).OfType<SpikesFlowerBodyClip>().Distinct())
            {
                Check(flower.Clip is MovieClip movie && movie.TotalFrames == 6 && movie.Stoped,
                    "Mango flower did not use the complete six-frame movie in its idle state.");
                Vector2 eyeOrigin = flower.Eye.LocalToNode(Vector2.Zero, game.Builder.GameRoot);
                Vector2 expectedOrigin = flower.Clip.LocalToNode(Vector2.Zero, game.Builder.GameRoot)
                    + Vector2.Transform(flower.Eye.Position, Matrix3x2.CreateRotation(flower.Clip.RotationRadians));
                Check(Vector2.Distance(eyeOrigin, expectedOrigin) < 0.01f,
                    "The flower eye's resolved position inherited the mouth scale.");
                float expectedScale = flower.Clip.ScaleY * 0.7f;
                Check(Math.Abs(Vector2.Distance(eyeOrigin, flower.Eye.LocalToNode(Vector2.UnitX, game.Builder.GameRoot)) - expectedScale) < 0.001f
                    && Math.Abs(Vector2.Distance(eyeOrigin, flower.Eye.LocalToNode(Vector2.UnitY, game.Builder.GameRoot)) - expectedScale) < 0.001f,
                    "The flower eye inherited the mouth scale a second time.");
            }
        }

        public static void Run(ContreJourGame game, Action<float> tick, Action<string> capture)
        {
            VerifyClosedHeroEye(game, capture);
            MovableSnotEye eye = game.Builder.World.BodyList.Select(body => body.UserData).OfType<MovableSnotEye>().Single();
            SnotPoint initial = game.SnotPoints.Single(point => point.Used);
            SnotPoint destination = game.SnotPoints.Single(point => !point.Used);
            Touch drag = new();
            drag.Initialize(new CursorPoint(game.Builder.GameRoot.LocalToGlobal(game.Builder.ToPoint(eye.Body.Position)), 7101, TouchType.Touch));
            Check(game.TouchBegin(drag), "Mango rejected the movable tentacle eye touch.");
            drag.Position = game.Builder.GameRoot.LocalToGlobal(game.Builder.ToPoint(destination.Body.Position));
            _ = game.TouchMove(drag);
            Step(tick, 60);
            capture("anchor-dragging");
            Check(Vector2.Distance(eye.Snot.StartPosition, eye.Body.Position) < 0.01f,
                "The tentacle mesh is still attached to its old anchor while dragging the eye.");
            game.TouchEnd(drag);
            Step(tick, 60);
            Check(destination.Used && !initial.Used, "Dragging the eye did not transfer the occupied anchor.");
            Check(eye.Snot.Enabled && eye.Snot.Physics.EyeJoint != null, "Releasing the eye did not restore its tentacle joint.");
            Check(Vector2.Distance(eye.Body.Position, destination.Body.Position) < 0.1f, "Released eye did not settle on its new anchor.");
            Check(Vector2.Distance(eye.Snot.StartPosition, eye.Body.Position) < 0.01f,
                "The tentacle mesh did not follow the relocated eye.");
            capture("anchor-moved");
            eye.Restart();
            Step(tick, 60);
            Check(initial.Used && !destination.Used && eye.Snot.Enabled, "Restart did not restore the original anchor.");
            Check(Vector2.Distance(eye.Body.Position, initial.Body.Position) < 0.1f, "Restarted eye did not settle on its original anchor.");
            capture("anchor-restored");
            game.OnMenuPressed();
            Step(tick, 60);
            Check(game.Paused, "Mango pause panel did not open.");
            capture("paused");
            game.OnMenuPressed();
            Step(tick, 60);
            Check(!game.Paused, "Mango did not resume after closing pause.");
        }

        public static void VerifyResultEye(FinishView finishView, Action<string> capture)
        {
            FakeHeroEye eye = Descendants(finishView).OfType<FakeHeroEye>().Single();
            foreach (string animation in new[] { "McFakeHeroEyeBlink", "McFakeHeroEyeSmile" })
            {
                eye.SetEyeContent(new EyeAnimation(animation, lockX: true, lockY: true));
                MovieClip background = (MovieClip)eye.CurrentBackground;
                background.GotoAndStop(background.TotalFrames / 2);
                background.Update(0f);
                eye.Update(0f);
                capture(animation);
                Sprite pupil = Descendants(eye).OfType<Sprite>().Single(sprite => sprite != background);
                Vector2 position = pupil.Position;
                // Render outside the eyelid to check the mask against pixels, including animated views.
                pupil.Position = new Vector2(0f, -100f);
                byte[] withPupil = RegressionApplication.CaptureFrame().Pixels;
                pupil.Visible = false;
                byte[] withoutPupil = RegressionApplication.CaptureFrame().Pixels;
                pupil.Visible = true;
                pupil.Position = position;
                Check(withPupil.SequenceEqual(withoutPupil), "The result-screen pupil draws outside its animated eyelid.");
            }
            eye.SetDefaultView();
            Sprite openPupil = Descendants(eye).OfType<Sprite>().Single(sprite => sprite != eye.CurrentBackground);
            byte[] openWithPupil = RegressionApplication.CaptureFrame().Pixels;
            openPupil.Visible = false;
            byte[] openWithoutPupil = RegressionApplication.CaptureFrame().Pixels;
            openPupil.Visible = true;
            Check(!openWithPupil.SequenceEqual(openWithoutPupil), "The result-screen mask hid the pupil in its open eye.");
        }

        private static void VerifyClosedHeroEye(ContreJourGame game, Action<string> capture)
        {
            HeroEye eye = game.Hero.Eye;
            eye.SetEyeContent(new EyeAnimation("McEyeClose", lockX: true, lockY: true));
            MovieClip background = (MovieClip)eye.CurrentBackground;
            background.GotoAndStop(background.TotalFrames / 2);
            background.Update(0f);
            eye.Update(0f);
            capture("hero-eye-half-closed");
            background.GotoAndStop(background.TotalFrames - 1);
            background.Update(0f);
            eye.Update(0f);
            Sprite pupil = Descendants(eye).OfType<Sprite>().Single(sprite =>
                ReferenceEquals(sprite.Texture, Mokus2DGame.LoadSpriteData("chapter6/McEyeBall_6").Texture));
            capture("hero-eye-closed");
            byte[] withPupil = RegressionApplication.CaptureFrame().Pixels;
            pupil.Visible = false;
            byte[] withoutPupil = RegressionApplication.CaptureFrame().Pixels;
            pupil.Visible = true;
            eye.SetDefaultView();
            Check(withPupil.SequenceEqual(withoutPupil), "The hero pupil draws over the closed eyelid.");
            byte[] openWithPupil = RegressionApplication.CaptureFrame().Pixels;
            pupil.Visible = false;
            byte[] openWithoutPupil = RegressionApplication.CaptureFrame().Pixels;
            pupil.Visible = true;
            Check(!openWithPupil.SequenceEqual(openWithoutPupil), "The mask hid the hero pupil in its open eye.");
        }

        private static System.Collections.Generic.IEnumerable<Node> Descendants(Node node)
        {
            foreach (Node child in node.Children)
            {
                yield return child;
                foreach (Node descendant in Descendants(child))
                {
                    yield return descendant;
                }
            }
        }

        private static void Step(Action<float> tick, int frames)
        {
            for (int frame = 0; frame < frames; frame++)
            {
                tick(1f / 60f);
            }
        }

        private static void Check(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
