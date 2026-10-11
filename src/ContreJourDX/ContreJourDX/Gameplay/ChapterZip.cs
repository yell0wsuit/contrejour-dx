using System.Numerics;

using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class ChapterZip(int index, MainMenu menu) : ChapterItem(index, menu)
    {
        private Tablo arrow;

        private Sprite highlite;

        private MovieClip openAnimation;

        protected override void CreateSprites()
        {
            openAnimation = new MovieClip("McPlanetZip");
            Background = openAnimation;
            arrow = new Tablo("McZipArrow");
            Container.AddChild(arrow);
            arrow.Position = new Vector2(-82f, 30f);
            AlphaItems.Add(arrow);
            BlurBackground = new Sprite("McPlanetZipBlur");
            openAnimation.Repeat = false;
            openAnimation.Stoped = true;
            openAnimation.Speed = 1.5f;
            Container.AddChild(Background);
            highlite = new Sprite("McZipHighlite");
            Container.AddChild(highlite);
            highlite.Visible = false;
        }

        public override void Update(float time)
        {
            base.Update(time);
            bool flag = Maths.FuzzyEquals(Depth, 1f, 0.01f);
            arrow.Open = flag;
            if (flag && openAnimation.CurrentFrame < openAnimation.MaxFrame)
            {
                openAnimation.Stoped = false;
                openAnimation.Rewind = false;
            }
            else if (!flag)
            {
                openAnimation.Rewind = true;
                if (openAnimation.CurrentFrame >= openAnimation.MaxFrame)
                {
                    openAnimation.CurrentFrame = openAnimation.MaxFrame;
                    openAnimation.Stoped = false;
                }
            }
            if (flag && !highlite.Visible && openAnimation.CurrentFrame >= openAnimation.MaxFrame)
            {
                highlite.Tweener.Stop();
                highlite.OpacityByte = 0;
                _ = highlite.Tweener.RepeatSequenceForever(1.5f).FadeIn().Next(1.5f)
                    .FadeOut();
                highlite.Visible = true;
            }
            else if (!flag)
            {
                highlite.Visible = false;
            }
        }
    }
}
