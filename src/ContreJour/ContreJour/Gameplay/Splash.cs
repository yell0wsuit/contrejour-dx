using System;

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

namespace ContreJour.Gameplay;

public class Splash : Node, ITouchListener, IDisposable
{
    public EventSender EndEvent { get; } = new();

    private Vector2 blackHeroPosition = new Vector2(360f + (W7IPhoneWidthDiff / 4f), 153f) * 2f;

    private LayerColor background;

    private Vector2 center;

    private McMokusLogo mokusLogo;

    private FakeHeroBlack blackHero;

    private bool ended;

    private static readonly float W7IPhoneWidthDiff = ScreenConstants.W7FromIPhoneSize.X - ScreenConstants.OsSizes.IPhoneRetina.X;

    private readonly Action[] afterLogo;

    private void InitializeAnimation()
    {
        ShowMokus();
        background.Color = Color.Black;
    }

    public Splash(params Action[] afterLogo)
    {
        this.afterLogo = afterLogo;
        _ = this.Schedule(0.1f, Begin);
    }

    private void Begin()
    {
        _ = new McFakeHeroEyeOpen();
        background = new LayerColor(Color.White, "menu/whitePixel");
        AddChild(background);
        center = ScreenConstants.W7FromIPhoneScreenCenter;
        Position = (ContreJourConfig.RootSize / 2f) - center;
        SplashStarted();
        InitializeAnimation();
        _ = this.Schedule(0.01f, StartAnimation);
    }

    private void StartAnimation()
    {
        AddListeners();
        _ = this.Schedule(0.9f, PlaySplashSound);
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

    private void ShowMokus()
    {
        mokusLogo = new McMokusLogo();
        AddChild(mokusLogo);
        mokusLogo.Position = center;
        _ = background.Tweener.Start(1f).Tween(NodeValues.Color, Color.Black);
        _ = this.Schedule(1.5f, MoveMokus);
    }

    private void MoveMokus()
    {
        blackHero = new FakeHeroBlack();
        AddChild(blackHero);
        blackHero.Position = blackHeroPosition + new Vector2(ScreenConstants.W7FromIPhoneSize.X / 2.5f, 0f);
        blackHero.Scale = 0.7f;
        _ = blackHero.Tweener.Start(1f).MoveTo(blackHeroPosition, Cubic.EaseOut);
        _ = blackHero.Background.Tweener.Start(1f).RotateTo(540.ToRadians(), Cubic.EaseOut);
        blackHero.SetMoveAngle(-(float)Math.PI, 3f);
        blackHero.SetViewAngle(-(float)Math.PI, 1f);
        _ = this.Schedule(1.2f, MoveMokusOut);
    }

    private void MoveMokusOut()
    {
        Vector2 vector = new(ScreenConstants.W7FromIPhoneSize.X * 0.8f, 0f);
        _ = blackHero.MoveTo(1f, blackHeroPosition + vector, Cubic.EaseIn);
        _ = blackHero.Background.RotateTo(1f, 0f - 540.ToRadians(), Cubic.EaseIn);
        blackHero.SetMoveAngle(0f, 3f);
        blackHero.SetViewAngle(0f, 1f);
        blackHero.Eye.EyeStep = 0.2f;
        _ = mokusLogo.MoveTo(1f, center + vector, Cubic.EaseIn);
        _ = this.Schedule(1.5f, End);
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
        LayerColor layerColor = new(Color.Black, "menu/whitePixel");
        AddChild(layerColor, 10);
        layerColor.OpacityByte = 0;
        _ = layerColor.FadeIn(0.3f).OnComplete(ShowHeadphones);
    }

    private void ShowHeadphones()
    {
        Node node = new();
        LayerColor layerColor = new(Color.Black, "menu/whitePixel");
        node.AddChild(layerColor);
        Sprite sprite = new McHeadphones
        {
            Position = ScreenConstants.W7FromIPhoneScreenCenter,
            IgnoreParentColor = true
        };
        layerColor.AddChild(sprite);
        Label label = ContreJourLabelUtil.CreateMultilineLabel(22f, "USE_HEADPHONES");
        label.Color = ContreJourConstants.GreyColor;
        sprite.AddChild(label);
        label.Position = new Vector2(-100f, -160f);
        label.Align = TextAlign.Center;
        label.AnchorX = 0.5f;
        node.OpacityByte = 0;
        AddChild(node, 10);
        _ = node.Tweener.StartSequence(0.5f).FadeIn().Next(2f)
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
