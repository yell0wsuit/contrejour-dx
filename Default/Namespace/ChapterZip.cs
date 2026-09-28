using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Default.Namespace;

public class ChapterZip(int index, MainMenu menu) : ChapterItem(index, menu)
{
    private Tablo arrow;

    private Sprite highlite;

    private MovieClip openAnimation;

    private Sprite shadow;

    protected override void CreateSprites()
    {
        openAnimation = new MovieClip("McPlanetZip");
        background = openAnimation;
        arrow = new Tablo("McZipArrow");
        container.AddChild(arrow);
        arrow.Position = new Vector2(-82f, 30f);
        alphaItems.Add(arrow);
        blurBackground = new Sprite("McPlanetZipBlur");
        openAnimation.Repeat = false;
        openAnimation.Stoped = true;
        openAnimation.Speed = 1.5f;
        container.AddChild(background);
        highlite = new Sprite("McZipHighlite");
        container.AddChild(highlite);
        highlite.Visible = false;
    }

    public override void Update(float time)
    {
        base.Update(time);
        bool flag = Maths.FuzzyEquals(depth, 1f, 0.01f);
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
