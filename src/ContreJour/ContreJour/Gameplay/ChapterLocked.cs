using System.Globalization;

using ContreJour.Clips.menu;
using ContreJour.Clips.planets;
using ContreJour.Utils;

using Microsoft.Xna.Framework;

using Mokus2D.Events;
using Mokus2D.Sound;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Particles.Util;
using Mokus2D.Visual.Text;

namespace ContreJour.Gameplay;

public class ChapterLocked : ChapterItem
{
    public EventSender ExplodeEvent { get; } = new();

    private bool exploding;

    private readonly Explosion explosion;

    private Tablo tablo;

    public ChapterItem TargetChapter { get; set; }

    public override float Depth
    {
        set
        {
            base.Depth = value;
            tablo.Open = value > 0.9f;
            if (!exploding && TargetChapter != null && value == 1f)
            {
                exploding = true;
                SoundManager.PlaySound("boom0", 0.7f);
                Actions.ShakeWithDurationOffsetCount(this, 0.4f, 5f, 20);
                _ = this.Schedule(0.4f, DoExplode);
            }
        }
    }

    public ChapterLocked(int index, MainMenu menu)
        : base(index, menu)
    {
        explosion = new Explosion("common/McSmokeBlack")
        {
            RotationDegrees = 90f,
            ScaleStep = -0.5f,
            HorizontalPosition = new RandomRange(0f, 20f),
            VerticalPosition = new RandomRange(0f, 20f),
            Speed = new RandomRange(220f, 60f),
            ParticlesScale = new RandomRange(1f, 0.5f)
        };
        explosion.CreateOnStartPosition(75);
    }

    protected override void CreateClickListener()
    {
    }

    public override void RemoveListeners()
    {
    }

    protected override void CreateSprites()
    {
        Background = new McPlanetLocked();
        Container.AddChild(Background);
        tablo = new Tablo
        {
            Position = new Vector2(-74f, 28f),
            Color = (Index == 1) ? ContreJourConstants.BlueLightColor : ContreJourConstants.GreyColor
        };
        Container.AddChild(tablo);
        Background.Color = tablo.Color;
        Label label = ContreJourLabelUtil.CreateLabel(20f, UserData.StarsToUnlock(Index).ToString(CultureInfo.CurrentCulture));
        label.Anchor = new Vector2(0.5f, 0.5f);
        label.Align = TextAlign.Left;
        label.Scale *= 0.8f;
        label.Color = Index == 1 ? Color.Lerp(ContreJourConstants.GreyColor, tablo.Color, 0.7f) : tablo.Color;
        tablo.AddChild(label);
        label.Position = new Vector2(60f, 46f);
        McEnergyIcon node = new();
        tablo.AddChild(node);
        node.Position = new Vector2(100f, 46f);
        McLockIcon sprite = new();
        tablo.AddChild(sprite);
        sprite.Position = new Vector2(24f, 46f);
        sprite.Color = label.Color;
        BlurBackground = new McChapterLockedBlur();
        HidingItems.Add(label);
        HidingItems.Add(tablo);
        HidingItems.Add(node);
    }

    protected override void OnClick()
    {
    }

    private void DoExplode()
    {
        _ = this.FadeOut(0.35f);
        _ = TargetChapter.FadeIn(0.3f);
        explosion.IgnoreParentOpacity = true;
        Parent.AddChild(explosion);
        explosion.Position = Position;
        _ = this.Schedule(1f, ExplodeEvent.SendEvent);
        _ = this.Schedule(1.5f, NodeValues.RemoveFromParent);
    }

    public override void Update(float time)
    {
        if (TargetChapter != null)
        {
            Depth = TargetChapter.Depth;
        }
        base.Update(time);
    }
}
