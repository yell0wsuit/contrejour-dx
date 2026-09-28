using System;
using System.Collections.Generic;

using ContreJour.Clips.fakeHero;
using ContreJour.Clips.loading;
using ContreJour.Config;
using ContreJour.Utils;

using ContreJourMono.ContreJour.Menu.LevelComplete;

using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Effects.Tween.Easing;
using Mokus2D.Events;
using Mokus2D.Input;
using Mokus2D.Sound;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Text;

namespace Default.Namespace;

public class Splash : Node, ITouchListener, IDisposable
{
    private const float JUMP_DURATION = 0.5f;

    public readonly EventSender EndEvent = new EventSender();

    protected Vector2 blackHeroPosition = new Vector2(360f + W7IPhoneWidthDiff / 4f, 153f) * 2f;

    protected LayerColor background;

    protected Sprite title;

    protected McChillingoLogo logo;

    protected FakeHero hero;

    protected Vector2 center;

    protected Sprite mokusLogo;

    protected FakeHeroBlack blackHero;

    private bool ended;

    private static readonly float W7IPhoneWidthDiff = ScreenConstants.W7FromIPhoneSize.X - ScreenConstants.OsSizes.IPhoneRetina.X;

    private static readonly Vector2 JUMP_OFFSET = new Vector2(0f, 100f);

    private static readonly Vector2 BLACK_HERO_POSITION = new Vector2(730f, 370f);

    private static readonly Vector2 HERO_POSITION_IPHONE = new Vector2(113f + W7IPhoneWidthDiff / 4f, 159f) * 2f;

    private static readonly Vector2 HERO_POSITION = new Vector2(241.3f, 382.9f);

    private static readonly Vector2 LOGO_POSITION_IPHONE = new Vector2(112.6f + W7IPhoneWidthDiff / 4f, 154.1f) * 2f;

    private static readonly Vector2 LOGO_POSITION = new Vector2(241.55f, 372.25f);

    private Action[] afterLogo;

    private void InitializeAnimation()
    {
        ShowMokus();
        background.Color = Color.Black;
    }

    public Splash(params Action[] afterLogo)
    {
        this.afterLogo = afterLogo;
        this.Schedule(0.1f, Begin);
    }

    private void Begin()
    {
        new McFakeHeroEyeOpen();
        background = new LayerColor(Color.White, "menu/whitePixel");
        AddChild(background);
        center = ScreenConstants.W7FromIPhoneScreenCenter;
        Position = ContreJourConfig.RootSize / 2f - center;
        SplashStarted();
        InitializeAnimation();
        this.Schedule(0.01f, StartAnimation);
    }

    private void InitializeChillingo()
    {
        title = new McChillingo();
        AddChild(title);
        title.Scale = 0.9375f;
        title.Position = new Vector2(W7IPhoneWidthDiff / 2f, -40f);
        AddLogo();
        this.Schedule(0.53f, Play);
    }

    private void StartAnimation()
    {
        AddListeners();
        this.Schedule(0.9f, PlaySplashSound);
    }

    private void AddLogo()
    {
        logo = new McChillingoLogo();
        AddChild(logo);
        logo.Stop();
        logo.Repeat = false;
        logo.startAnimation.Stop();
        logo.startAnimation.Repeat = false;
        logo.Scale = 0.9375f;
        logo.Position = LOGO_POSITION_IPHONE;
        hero = new FakeHero();
        hero.Scale = 0.9375f;
        hero.OpacityByte = 0;
        hero.Visible = false;
        AddChild(hero);
        hero.Position = HERO_POSITION_IPHONE;
    }

    private void AddListeners()
    {
        Mokus2DGame.Instance.KeysController.AddBackKeyListener(OnBackClick);
        Mokus2DGame.Instance.TouchController.AddListener(this);
    }

    private void SplashStarted()
    {
        afterLogo.Each(delegate (Action action)
        {
            action();
        });
    }

    private void PlaySplashSound()
    {
        UserData.Instance.RefreshSoundManager();
        if (!ended)
        {
            SoundManager.PlaySound(Sounds.IntroSound);
        }
    }

    private void OnBackClick()
    {
        StopSplashSound();
        End();
    }

    private void RemoveListeners()
    {
        if (logo != null)
        {
            logo.EndEvent -= ShowHero;
        }
        Mokus2DGame.Instance.KeysController.RemoveBackKeyListener(OnBackClick);
        Mokus2DGame.Instance.TouchController.RemoveListener(this);
    }

    public bool TouchBegin(Touch touch)
    {
        StopSplashSound();
        End();
        return false;
    }

    public bool TouchMove(Touch touch)
    {
        return false;
    }

    public void TouchEnd(Touch touch)
    {
    }

    private void Play()
    {
        logo.Play();
        logo.startAnimation.Play();
        logo.startAnimation.EndEvent += ShowHero;
    }

    private void ShowHero(IAnimatedNode animatedNode)
    {
        hero.Visible = true;
        hero.FadeIn(0.5f);
        this.Schedule(0.5f, EndJump);
        hero.Eye.Open();
    }

    private void EndJump()
    {
        logo.Visible = false;
        hero.SetViewAngle((float)Math.PI / 4f, 1f);
        this.Schedule(0.3f, LookRight);
    }

    private void LookRight()
    {
        hero.SetViewAngle((float)Math.PI * 3f / 4f, 1f);
        this.Schedule(0.3f, StartMove);
    }

    private void StartMove()
    {
        Vector2 vector = new Vector2(ScreenConstants.W7FromIPhoneSize.X, 0f);
        hero.Tweener.Start(2f).MoveTo(hero.Position + vector, Cubic.EaseIn);
        hero.SetViewAngle(0f, 0f);
        hero.SetMoveAngle(0f, 1f);
        title.Tweener.Start(2f).MoveTo(title.Position + vector, Cubic.EaseIn);
        hero.Background.Tweener.Start(2f).RotateTo(0f - 1080.ToRadians(), Cubic.EaseIn);
        this.Schedule(0.6f, RefreshSpeed);
        this.Schedule(0.1f, SlowLookRight);
        this.Schedule(2f, ShowMokus);
    }

    private void SlowLookRight()
    {
        hero.Eye.EyeStep = 0.4f;
        hero.SetViewAngle(0f, 1f);
    }

    private void RefreshSpeed()
    {
        hero.Speed = 6f;
    }

    private void ShowMokus()
    {
        if (logo != null)
        {
            RemoveChild(logo);
            RemoveChild(title);
            hero.Visible = false;
        }
        mokusLogo = new McMokusLogo();
        AddChild(mokusLogo);
        mokusLogo.Position = center;
        background.Tweener.Start(1f).Tween(NodeValues.Color, Color.Black);
        this.Schedule(1.5f, MoveMokus);
    }

    private void MoveMokus()
    {
        blackHero = new FakeHeroBlack();
        AddChild(blackHero);
        blackHero.Position = blackHeroPosition + new Vector2(ScreenConstants.W7FromIPhoneSize.X / 2.5f, 0f);
        blackHero.Scale = 0.7f;
        blackHero.Tweener.Start(1f).MoveTo(blackHeroPosition, Cubic.EaseOut);
        blackHero.Background.Tweener.Start(1f).RotateTo(540.ToRadians(), Cubic.EaseOut);
        blackHero.SetMoveAngle(-(float)Math.PI, 3f);
        blackHero.SetViewAngle(-(float)Math.PI, 1f);
        this.Schedule(1.2f, MoveMokusOut);
    }

    private void MoveMokusOut()
    {
        Vector2 vector = new Vector2(ScreenConstants.W7FromIPhoneSize.X * 0.8f, 0f);
        blackHero.MoveTo(1f, blackHeroPosition + vector, Cubic.EaseIn);
        blackHero.Background.RotateTo(1f, 0f - 540.ToRadians(), Cubic.EaseIn);
        blackHero.SetMoveAngle(0f, 3f);
        blackHero.SetViewAngle(0f, 1f);
        blackHero.Eye.EyeStep = 0.2f;
        mokusLogo.MoveTo(1f, center + vector, Cubic.EaseIn);
        this.Schedule(1.5f, End);
    }

    private static void StopSplashSound()
    {
        SoundManager.StopAllSounds();
        UserData.Instance.RefreshSoundManager();
    }

    public void End()
    {
        if (!ended)
        {
            StopSplashSound();
            RemoveListeners();
            HideAll();
            ended = true;
        }
    }

    private void HideAll()
    {
        LayerColor layerColor = new LayerColor(Color.Black, "menu/whitePixel");
        AddChild(layerColor, 10);
        layerColor.OpacityByte = 0;
        layerColor.FadeIn(0.3f).OnComplete(ShowHeadphones);
    }

    private void ShowHeadphones()
    {
        Node node = new Node();
        LayerColor layerColor = new LayerColor(Color.Black, "menu/whitePixel");
        node.AddChild(layerColor);
        Sprite sprite = new McHeadphones();
        sprite.Position = ScreenConstants.W7FromIPhoneScreenCenter;
        sprite.IgnoreParentColor = true;
        layerColor.AddChild(sprite);
        Label label = ContreJourLabelUtil.CreateMultilineLabel(22f, "USE_HEADPHONES");
        label.Color = ContreJourConstants.GREY_COLOR;
        sprite.AddChild(label);
        label.Position = new Vector2(-100f, -160f);
        label.Align = TextAlign.Center;
        label.AnchorX = 0.5f;
        node.OpacityByte = 0;
        AddChild(node, 10);
        node.Tweener.StartSequence(0.5f).FadeIn().Next(2f)
            .OnComplete((Action)delegate
            {
                EndEvent.SendEvent();
            });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        EndEvent.RemoveListeners();
    }
}
