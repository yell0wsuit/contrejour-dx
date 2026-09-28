using System;
using System.Collections.Generic;
using System.Globalization;

using ContreJour.Clips.menu;
using ContreJour.Config;
using ContreJour.Utils;

using ContreJourMono.ContreJour.Menu.LevelComplete;

using Microsoft.Xna.Framework;

using Mokus2D.Effects.Tween.Easing;
using Mokus2D.Events;
using Mokus2D.Sound;
using Mokus2D.Util;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;
using Mokus2D.Visual.Text;
using Mokus2D.Visual.Util;

namespace Default.Namespace;

public class FinishView : MovieStripesView, IDisposable
{
    private static readonly Vector2 STARS_OFFSET = new(100f, 120f);

    private static readonly Vector2 PORTAL_OFFSET = new(0f, 0f);

    private static readonly Vector2 HERO_OFFSET = new(-180f, -70f);

    private readonly Color BLUE_LIGHT_COLOR = ContreJourConstants.BLUE_LIGHT_COLOR;

    private readonly Color GREY_COLOR = ContreJourConstants.GREY_COLOR;

    public readonly EventSender NextLevelEvent = new();

    protected List<Button> buttons = [];

    protected Vector2 center;

    protected ClickableLayer clickableLayer;

    protected Color color;

    protected List<Sprite> energies;

    protected ContreJourGame game;

    protected FakeHero hero;

    protected Sprite highlite;

    protected Label levelField;

    protected LevelPosition levelPosition;

    protected bool newHighScore;

    protected MenuPortal portal;

    protected int score;

    private Button skipButton;

    protected Sprite stamp;

    protected int stars;

    protected ProgressLabel starsBonusField;

    protected float time;

    protected ProgressLabel totalField;

    public FinishView(ContreJourGame game)
        : base(game.BlackSide, fade: true)
    {
        this.game = game;
        this.game.Schedule(CacheTextures, 0.1f);
        Scale = 1.1f;
        Position = -ContreJourConfig.RootSize * 0.05f;
        CreateHero();
    }

    private void CacheTextures()
    {
        if (!game.BlackSide && !game.WhiteSide)
        {
            _ = game.BonusChapter;
        }
    }

    public void Show(LevelPosition level, int stars, int score, float time, bool newHighScore)
    {
        Show();
        this.newHighScore = newHighScore;
        this.stars = stars;
        this.score = score;
        this.time = time;
        levelPosition = level;
        _ = this.Schedule(FinishDuration, OnShow);
    }

    private void OnShow()
    {
        bool flag = levelPosition.Chapter == 1;
        this.color = flag ? BLUE_LIGHT_COLOR : GREY_COLOR;
        Color color = flag ? Color.Lerp(Color.White, BLUE_LIGHT_COLOR, 0.9f) : GREY_COLOR;
        Color color2 = flag ? BLUE_LIGHT_COLOR : ColorUtil.Mult(GREY_COLOR, 0.7f);
        if (game.BonusChapter)
        {
            this.color = ContreJourConstants.GreenLightColor;
            color = this.color;
            color2 = ContreJourConstants.GreenLightColor * 0.5f;
        }
        McLevelComplete sprite = new();
        AddChild(sprite);
        Vector2 rootSize = ContreJourConfig.RootSize;
        center = new Vector2(rootSize.X / 2f, rootSize.Y / 2f);
        sprite.Position = center;
        _ = sprite.FadeIn(0.1f);
        sprite.Color = color2;
        sprite.Scale = 1.2f;
        clickableLayer = new ClickableLayer
        {
            Position = sprite.Position
        };
        AddChild(clickableLayer);
        Button button = Button.ButtonBigWithIcon("menu/McRestartIcon");
        button.TouchEndEvent += delegate
        {
            RestartEvent.SendEvent();
        };
        button.Position = new Vector2(-5f, -116f);
        Button button2 = Button.ButtonBigWithIcon("menu/McMenuIcon");
        button2.TouchEndEvent += delegate
        {
            MenuEvent.SendEvent();
        };
        button2.Position = new Vector2(85f, -116f);
        buttons = [button, button2];
        if (levelPosition.SkipAvailable)
        {
            skipButton = Button.ButtonBigWithIcon("menu/McSkipIcon");
            skipButton.TouchEndEvent += delegate
            {
                NextLevelEvent.SendEvent();
            };
            skipButton.Position = new Vector2(185f, -110f);
            skipButton.RealScale = 1.2f;
            buttons.Add(skipButton);
        }
        for (int num = 0; num < buttons.Count; num++)
        {
            Button button3 = buttons[num];
            button3.TouchEndEvent += OnButtonClick;
            clickableLayer.AddChild(button3);
            button3.Color = color;
            button3.Visible = false;
            _ = this.Schedule((num + 2) * 0.1f, delegate
            {
                ShowItemWithButton(button3);
            }, null);
        }
        if (!game.BonusChapter)
        {
            skipButton.Color = ColorUtil.Mult(color, 1.2f);
        }
        energies = [];
        for (int num2 = 0; num2 < 3; num2++)
        {
            Sprite energy = (num2 < stars) ? new McEnergyBig() : new McEnergyBigInactive();
            energy.OpacityByte = (num2 < stars) ? 255 : 70;
            energy.Scale = 1.7f;
            energy.Visible = false;
            clickableLayer.AddChild(energy);
            energy.Position = STARS_OFFSET + new Vector2(100 * (num2 - 1), 0f);
            energies.Add(energy);
            _ = this.Schedule(num2 * 0.1f, delegate
            {
                ShowItemWithButton(energy);
            }, null);
            if (num2 < stars)
            {
                _ = this.Schedule(1f + (num2 * 0.5f), delegate
                {
                    BlinkItemWithButton(energy);
                }, null);
            }
        }
        Label label = ContreJourLabelUtil.CreateLabel(22f, Messages.CompleteText(stars));
        label.Color = this.color;
        label.Position = new Vector2(-154f, 120f);
        clickableLayer.AddChild(label);
        _ = label.FadeIn(0.2f);
        levelField = ContreJourLabelUtil.CreateLabel(18f, string.Format(CultureInfo.CurrentCulture, Messages.LevelFormat, levelPosition.Chapter + 1, levelPosition.Index + 1));
        clickableLayer.AddChild(levelField);
        levelField.Color = this.color;
        levelField.Anchor = new Vector2(0f, 0.5f);
        levelField.Position = new Vector2(-30f, 46f);
        levelField.OpacityByte = 0;
        _ = this.Schedule(0.5f, ShowLevel);
        highlite = new McHeroHighliteMenu
        {
            Scale = 10f,
            OpacityFloat = 0.4f
        };
        if (flag)
        {
            highlite.Color = BLUE_LIGHT_COLOR;
            highlite.OpacityByte = 140;
        }
        else if (game.BonusChapter)
        {
            highlite.Color = ContreJourConstants.GreenLightColor;
        }
        AddChild(highlite);
        highlite.Position = center + HERO_OFFSET;
        highlite.Visible = false;
        portal = new MenuPortal(Vector2.Zero);
        AddChild(portal);
        portal.Position = center + HERO_OFFSET + PORTAL_OFFSET;
        portal.Visible = false;
        portal.ItemsScale = 0f;
        portal.Scale = 2f;
        portal.ScaleStep = 0.2f;
        _ = this.Schedule(0.5f, ShowPortal);
        if (levelPosition.Chapter == 1)
        {
            hero.HotSpot.Color = ColorUtil.Mult(BLUE_LIGHT_COLOR, 1.5f);
        }
        hero.Position = portal.Position;
    }

    private void CreateHero()
    {
        hero = (FakeHero)ReflectUtil.CreateInstance(game.ChooseSide(typeof(FakeHeroBlack), typeof(FakeHeroWhite), typeof(FakeHero), typeof(FakeHero), typeof(FakeHeroGreen)));
        hero.Visible = false;
        AddChild(hero, 1);
    }

    private void OnButtonClick(TouchArguments touchArguments)
    {
        clickableLayer.InteractionsEnabled = false;
    }

    private void ShowLevel()
    {
        _ = levelField.FadeIn(0.5f);
        _ = this.Schedule(0.4f, ShowStarsBonus);
    }

    private static void PlayBell()
    {
        SoundManager.PlaySound("bell", 0.6f);
    }

    public void ShowPortal()
    {
        portal.Visible = true;
        portal.TargetScale = 1f;
        _ = this.Schedule(0.3f, ShowHero);
    }

    private void ShowHero()
    {
        hero.Visible = true;
        portal.TargetScale = 0f;
        portal.ScaleStep = 0.05f;
        hero.Scale = 0f;
        _ = hero.ScaleTo(0.3f, 1f);
        float opacityFloat = highlite.OpacityFloat;
        highlite.Visible = true;
        highlite.OpacityByte = 0;
        _ = highlite.FadeTo(0.2f, opacityFloat);
        _ = this.Schedule(0.5f, LookAtStar);
    }

    private void LookAtStar()
    {
        hero.ViewTarget = energies[1].LocalToNode(Vector2.Zero, this);
        _ = this.Schedule(0.7f, LookAtScore);
    }

    private void LookAtScore()
    {
        hero.ViewTarget = totalField.LocalToNode(Vector2.Zero, this);
        if (newHighScore)
        {
            UserData.Instance.Improved = true;
            _ = this.Schedule(0.7f, LookAtImproved);
        }
        else
        {
            _ = this.Schedule(0.5f, LookAtPlayer);
        }
    }

    private void LookAtImproved()
    {
        hero.ViewTarget = stamp.LocalToNode(Vector2.Zero, this);
        _ = this.Schedule(0.5f, LookAtPlayer);
    }

    private void LookAtPlayer()
    {
        hero.ViewTarget = hero.Position;
        _ = newHighScore || stars == 3 ? this.Schedule(0.5f, Smile) : this.Schedule(0.5f, Blink);
    }

    private void Blink()
    {
        hero.Eye.Blink();
        _ = this.Schedule(Maths.Random(3f, 7f), Blink);
    }

    private void Smile()
    {
        SoundManager.PlaySound("laugh0", 0.8f);
        hero.Eye.Smile();
        _ = this.Schedule(Maths.Random(3f, 7f), Blink);
    }

    private void ShowStarsBonus()
    {
        starsBonusField = ContreJourLabelUtil.CreateProgressLabel(15f, Messages.ENERGY_BONUS, stars * 1000, 15);
        starsBonusField.Position = new Vector2(-30f, 16f);
        clickableLayer.AddChild(starsBonusField);
        _ = starsBonusField.FadeIn(0.2f);
        starsBonusField.Color = color;
        starsBonusField.Anchor = new Vector2(0f, 0.5f);
        _ = this.Schedule(0.4f, ShowTimeBonus);
    }

    private void ShowTimeBonus()
    {
        ProgressLabel progressLabel = ContreJourLabelUtil.CreateProgressLabel(15f, Messages.TIME_BONUS, UserData.GetTimeBonus(time), 15);
        clickableLayer.AddChild(progressLabel);
        progressLabel.Position = new Vector2(-30f, -12f);
        _ = progressLabel.FadeIn(0.2f);
        progressLabel.Color = color;
        progressLabel.Anchor = new Vector2(0f, 0.5f);
        _ = this.Schedule(0.4f, ShowLine);
        _ = this.Schedule(0.6f, ShowTotal);
    }

    private void ShowLine()
    {
        McTotalLine mcTotalLine = new()
        {
            Color = color
        };
        clickableLayer.AddChild(mcTotalLine);
        mcTotalLine.Speed = 2f;
        mcTotalLine.Position = new Vector2(-30f, -26f);
        mcTotalLine.Repeat = false;
    }

    private void ShowTotal()
    {
        totalField = ContreJourLabelUtil.CreateProgressLabel(18f, Messages.TOTAL, score, 15);
        clickableLayer.AddChild(totalField);
        totalField.Position = new Vector2(-30f, -50f);
        _ = totalField.FadeIn(0.2f);
        totalField.Anchor = new Vector2(0f, 0.5f);
        totalField.Color = color;
        if (newHighScore)
        {
            _ = this.Schedule(0.7f, ShowNewHighScore);
        }
    }

    private void ShowNewHighScore()
    {
        stamp = new McImprovedResult();
        Label label = ContreJourLabelUtil.CreateMultilineLabel(20f, "IMPROVED_RESULT");
        label.AnchorY = 0.5f;
        label.LineSpacing = -10f;
        label.Position = new Vector2(56f, -82f);
        label.RotationDegrees = 10.8f;
        stamp.AddChild(label);
        stamp.Scale = 0f;
        stamp.Position = new Vector2(-270f, 100f);
        stamp.RotationDegrees = -170f;
        _ = stamp.ScaleTo(0.25f, 0.7f, Cubic.EaseIn);
        _ = stamp.RotateTo(0.4f, 0f, Cubic.EaseOut);
        clickableLayer.AddChild(stamp);
    }

    private static void BlinkItemWithButton(Sprite button)
    {
        _ = button.Tweener.StartSequence(0.15f).ScaleTo(button.Scale * 1.2f, Cubic.EaseOut).Next(0.15f)
            .ScaleTo(button.Scale, Elastic.EaseIn);
        PlayBell();
    }

    private static void ShowItemWithButton(Sprite button)
    {
        float scale = button.Scale;
        button.Scale = 0f;
        button.Visible = true;
        float opacityFloat = button.OpacityFloat;
        button.OpacityByte = 0;
        _ = button.FadeTo(0.3f, opacityFloat);
        _ = button.ScaleTo(0.6f, scale, Elastic.EaseOut);
    }
}
