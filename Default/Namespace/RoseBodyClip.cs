using System.Collections.Generic;
using System.Linq;

using ContreJour.Clips.level1;
using ContreJour.Config;

using Default.Namespace.Rose;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Sound;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;

namespace Default.Namespace;

public class RoseBodyClip : StickyBodyClip, IBonusAcceptable, IBodyClip
{
    private static readonly Vector2 HeroJumpImpulse = new(0f, 2f);

    private static readonly Vector2 PuddleOffset = new(-80f, -1f);

    private bool bonusHidden;

    private readonly FinalRose finalRose;

    private bool finished;

    private readonly ContreJourGame game;

    private McRoseHeadBack headBack;

    private McRoseHeadDown headDown;

    private McRoseHeadFront headFront;

    private McRoseHeadLight headLight;

    private IntroPlayer intro;

    private McLystok1 leaf1;

    private McLystok2 leaf2;

    private McLystokMain leafMain;

    private McPuddle puddle;

    private List<IAnimatedNode> roseParts;

    private Button skipButton;

    private MovieClip stalk;

    public RoseBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        Node node = new();
        LevelBuilderBase.ReplaceChildWith(clip, node);
        node.Position = clip.Position;
        this.Clip = node;
        game = (ContreJourGame)builder.Game;
        finalRose = new FinalRose
        {
            Stoped = true,
            Repeat = false,
            Speed = 0f
        };
        game.BonusTarget = this;
        this.Clip.AddChild((Node)finalRose);
        if (game.CanShowIntro)
        {
            PlayIntro();
        }
        else
        {
            finished = true;
        }
    }

    public Vector2 BonusTarget()
    {
        return Clip.Position + new Vector2(0f, 30f);
    }

    public void ApplyBonus()
    {
        finalRose.Stoped = false;
        if (finalRose.CurrentFrame < 1f)
        {
            finalRose.Speed = 0.3f;
        }
        else if (finalRose.CurrentFrame < 30f)
        {
            finalRose.Speed += 0.3f;
        }
    }

    private void OnSkipClick(TouchArguments touchArguments)
    {
        if (!game.Paused)
        {
            game.Restart();
        }
    }

    public void PlayIntro()
    {
        skipButton = new Button("common/McSkipIcon");
        skipButton.TouchEndEvent += OnSkipClick;
        skipButton.RealScale = 1.3f;
        skipButton.Icon.Scale = 0.75f;
        skipButton.Color = game.ButtonsColor;
        game.ClickableLayer.AddChild(skipButton);
        skipButton.Position = ContreJourConfig.BackButtonPosition;
        if (ContreJourConfig.BackButtonVisible)
        {
            skipButton.Y = ContreJourConfig.RootSize.Y - ContreJourConfig.BackButtonPosition.Y;
        }
        intro = new IntroPlayer(game);
        _ = Builder.AddChild(intro);
        stalk = new McStebloAnimation();
        headLight = new McRoseHeadLight();
        headBack = new McRoseHeadBack();
        headFront = new McRoseHeadFront();
        headDown = new McRoseHeadDown();
        headDown.content.light.Visible = false;
        headDown.content.IgnoreAnimations("light");
        leafMain = new McLystokMain();
        leafMain.tear.Stop();
        leafMain.tear.Repeat = false;
        leaf1 = new McLystok1
        {
            Repeat = false,
            Stoped = true,
            Position = Clip.Position,
            Speed = 0.5f
        };
        _ = Builder.AddChild(leaf1);
        leaf2 = new McLystok2();
        ((Node)finalRose).Visible = false;
        roseParts = [stalk, headDown, leaf2];
        puddle = new McPuddle
        {
            Position = Clip.Position + PuddleOffset,
            Repeat = false,
            Visible = false,
            Stoped = true,
            Speed = 0.7f
        };
        _ = Builder.AddChild(puddle);
        foreach (IAnimatedNode rosePart in roseParts)
        {
            rosePart.Repeat = false;
            rosePart.Stoped = true;
            Clip.AddChild((Node)rosePart);
        }
        _ = Builder.AddChild(leafMain);
        leafMain.Repeat = false;
        leafMain.Stoped = true;
        leafMain.Position = Clip.Position;
        Clip.AddChild(headBack);
        Clip.AddChild(headLight);
        Clip.AddChild(headFront);
        leafMain.MaxFrame = 79f;
        headDown.VisibleAndUpdating = false;
        headDown.content.Repeat = false;
        leaf1.EndEvent += OnLeaf1Fall;
        game.TouchEnabled = false;
        headLight.OpacityByte = 150;
        _ = headLight.Tweener.StartSequence(2.5f).FadeTo(10f / 51f).Next(2.5f)
            .FadeTo(20f / 51f)
            .Next(2.5f)
            .FadeTo(0.11764706f)
            .Next(2.5f)
            .FadeTo(16f / 51f)
            .Next(2.5f)
            .FadeOut()
            .OnComplete(GoDown);
    }

    private void OnLeaf1Fall(IAnimatedNode animatedNode)
    {
        leafMain.MaxFrame = leafMain.TotalFrames;
        leafMain.Play();
        leafMain.tear.Play();
        leafMain.EndEvent += OnLeafMainEnd;
    }

    private void OnLeafMainEnd(IAnimatedNode animatedNode)
    {
        leafMain.EndEvent += OnLeafMainEnd;
        Schedule(ShowPuddle, 1f);
    }

    private void ShowPuddle()
    {
        puddle.Visible = true;
        puddle.Stoped = false;
        puddle.EndEvent += OnPuddleEnd;
    }

    private void OnPuddleEnd(IAnimatedNode animatedNode)
    {
        SoundManager.PlaySound("begin5", 0.5f);
        puddle.EndEvent -= OnPuddleEnd;
        game.Hero.Body.BodyType = (BodyType)2;
        game.Hero.SetPosition(puddle.Position);
        game.Hero.Body.SetSensor(value: true);
        game.Hero.Clip.Visible = true;
        game.Hero.EyeAnimationsAllowed = false;
        game.Hero.Body.ApplyLinearImpulse(HeroJumpImpulse, game.Hero.Body.WorldCenter);
        game.Hero.Body.FixedRotation = true;
        Schedule(OnHeroJump, 0.5f);
        Schedule(LookAtRose, 2.5f);
    }

    private void OnHeroJump()
    {
        game.Hero.Body.SetSensor(value: false);
        puddle.Rewind = true;
        puddle.Stoped = false;
    }

    private void LookAtRose()
    {
        game.Hero.EyeMoveAllowed = false;
        game.Hero.SetEyeTargetAngle(MathHelper.ToRadians(30f));
        Schedule(LookAtBonuses, 1.5f);
        Schedule(game.ShowBonuses, 1f);
    }

    private void LookAtBonuses()
    {
        game.Hero.SetEyeTargetAngle(MathHelper.ToRadians(160f));
        Schedule(FinishMovie, 1.5f);
        skipButton.TouchEndEvent -= OnSkipClick;
        _ = skipButton.FadeOutAndHide(0.3f);
        finished = true;
        ((Node)finalRose).Visible = true;
        foreach (Node rosePart in roseParts.Cast<Node>())
        {
            Clip.RemoveChild(rosePart);
        }
        Builder.RemoveChild(leaf1);
        Builder.RemoveChild(leafMain);
        Clip.RemoveChild(headFront);
        Clip.RemoveChild(headBack);
        Clip.RemoveChild(headLight);
        Builder.RemoveChild(puddle);
    }

    private void FinishMovie()
    {
        game.Hero.EyeAnimationsAllowed = true;
        game.Hero.EyeMoveAllowed = true;
        game.Hero.Body.FixedRotation = false;
        game.TouchEnabled = true;
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (finished)
        {
            if (finalRose.Speed > -1f)
            {
                finalRose.Speed -= time / 20f * finalRose.CurrentFrame;
            }
            finalRose.Stoped = false;
        }
        if (game.CanShowIntro && !bonusHidden && time > 0f)
        {
            game.Hero.EyeAnimationsAllowed = false;
            game.Hero.Clip.Visible = false;
            game.HideBonuses();
            bonusHidden = true;
        }
    }

    private void GoDown()
    {
        headFront.Visible = false;
        headBack.Visible = false;
        headLight.Visible = false;
        headDown.VisibleAndUpdating = true;
        foreach (IAnimatedNode rosePart in roseParts)
        {
            rosePart.Stoped = false;
        }
        leaf1.Play();
        leafMain.Play();
    }
}
