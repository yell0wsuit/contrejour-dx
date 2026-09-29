using System;
using System.Collections.Generic;
using System.Globalization;

using ContreJour.Clips.menu;
using ContreJour.Config;
using ContreJour.Utils;

using Microsoft.Xna.Framework;

using Mokus2D.Effects.Tween.Easing;
using Mokus2D.Sound;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;
using Mokus2D.Visual.Text;
using Mokus2D.Visual.Util;

namespace Default.Namespace;

public class PausePanel : Node, IDisposable
{
    private readonly LayerColor backgroundLayer = new(Color.Black, "menu/whitePixel");

    private int buttonIndex;

    private readonly List<Button> buttons;

    private readonly ClickableLayer clickableLayer;

    private readonly ContreJourGame game;

    private readonly Label levelLabel;

    private readonly ToggleButton musicButton;

    private bool open;

    private readonly Button restartButton;

    private readonly Label scoreLabel;

    private readonly ToggleButton soundButton;

    private Vector2 winSize;

    public PausePanel(ContreJourGame game)
    {
        this.game = game;
        SoundManager.MusicDisableEvent += OnMusicDisable;
        Color color = this.game.BlackSide ? ColorUtil.Mult(ContreJourConstants.BlueLightColor, 1.5f) : ContreJourConstants.GreyColor;
        if (this.game.BonusChapter)
        {
            color = ContreJourConstants.GreenLightColor;
        }
        winSize = ContreJourConfig.RootSize;
        Position = new Vector2(winSize.X, 0f);
        AddChild(backgroundLayer);
        backgroundLayer.OpacityByte = 0;
        McRightPanelBackground sprite = new();
        AddChild(sprite);
        sprite.Scale = Math.Max((winSize.Y + 10f) / sprite.Size.Y * 1.4f, 358f / sprite.Size.X);
        sprite.Position = new Vector2(sprite.ScaledSize.X + -306f - 52f, (0f - sprite.Size.Y) * 0.2f);
        sprite.Color = color;
        clickableLayer = new ClickableLayer();
        AddChild(clickableLayer);
        float realScale = 1.2f;
        Button button = Button.ButtonBigWithIcon("menu/McPlayIcon");
        clickableLayer.AddChild(button);
        button.TouchEndEvent += OnPlayClick;
        button.Position = new Vector2(-306f, winSize.Y / 2f);
        button.Color = color;
        button.RealScale = realScale;
        restartButton = Button.ButtonBigWithIcon("menu/McRestartIcon");
        clickableLayer.AddChild(restartButton);
        restartButton.TouchEndEvent += OnRestartClick;
        float num = -156f;
        restartButton.Position = new Vector2(num, (winSize.Y / 2f) + 120f);
        restartButton.Color = color;
        restartButton.RealScale = realScale;
        Button button2 = Button.ButtonBigWithIcon("menu/McMenuIcon");
        clickableLayer.AddChild(button2);
        button2.TouchEndEvent += OnMenuClick;
        button2.Position = new Vector2(num, (winSize.Y / 2f) + 0f);
        button2.Color = color;
        button2.RealScale = realScale;
        Button button3 = Button.ButtonBigWithIcon("menu/McSkipIcon");
        clickableLayer.AddChild(button3);
        button3.TouchEndEvent += OnSkipClick;
        button3.Position = new Vector2(num, (winSize.Y / 2f) - 120f);
        button3.Color = color;
        button3.RealScale = realScale;
        soundButton = new ToggleButton("menu/McSoundIcon", "menu/McDisabledIcon");
        clickableLayer.AddChild(soundButton);
        soundButton.Position = new Vector2(num + -60f, (winSize.Y / 2f) - -240f);
        soundButton.Color = color;
        soundButton.RealScale = 1.4f;
        soundButton.ToggleIcon.IgnoreParentColor = true;
        musicButton = new ToggleButton("menu/McMusicIcon", "menu/McDisabledIcon");
        clickableLayer.AddChild(musicButton);
        musicButton.Position = new Vector2(num + 60f, (winSize.Y / 2f) - -240f);
        musicButton.Color = color;
        musicButton.ToggleIcon.IgnoreParentColor = true;
        musicButton.RealScale = 1.4f;
        soundButton.Visible = false;
        musicButton.Visible = false;
        RefreshSoundButtons();
        soundButton.TouchEndEvent += OnSoundClick;
        musicButton.TouchEndEvent += OnMusicClick;
        buttons = [button, restartButton, button2, button3];
        scoreLabel = ContreJourLabelUtil.CreateLabel(15f);
        scoreLabel.Position = new Vector2(-160f, 30f);
        AddChild(scoreLabel);
        scoreLabel.Color = ColorUtil.Mult(color, 0.5f);
        scoreLabel.OpacityByte = 0;
        scoreLabel.Visible = false;
        levelLabel = ContreJourLabelUtil.CreateLabel(15f);
        levelLabel.Position = new Vector2(-160f, 70f);
        AddChild(levelLabel);
        levelLabel.Color = scoreLabel.Color;
        levelLabel.OpacityByte = 0;
        levelLabel.Visible = false;
        Visible = false;
        Position = new Vector2(winSize.X + 300f, 0f);
        SetButtonsVisible(value: false);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        SoundManager.MusicDisableEvent -= OnMusicDisable;
    }

    private void OnMusicDisable()
    {
        musicButton.Toggle = true;
    }

    public void SetLevelIndex(int levelIndex)
    {
        if (levelIndex == 169)
        {
            levelLabel.Visible = false;
            scoreLabel.Visible = false;
            _ = buttons.RemoveLast();
            return;
        }
        LevelData levelDataByFile = UserData.Instance.GetLevelDataByFile(levelIndex);
        LevelPosition levelPosition = UserData.GetLevelPosition(levelIndex);
        if (levelPosition.Chapter == Constants.NormalChaptersCount - 1 && levelPosition.Index == 19 && UserData.Instance.GetLevelDataByPosition(levelPosition) == null)
        {
            _ = buttons.RemoveLast();
            restartButton.Visible = false;
        }
        if (!levelPosition.SkipAvailable)
        {
            _ = buttons.RemoveLast();
            restartButton.Visible = false;
        }
        string textString = string.Format(CultureInfo.CurrentCulture, Messages.LevelFormat, levelPosition.Chapter + 1, levelPosition.Index + 1, null);
        levelLabel.TextString = textString;
        _ = scoreLabel.AppendFormat(Messages.BestScore, levelDataByFile?.Score ?? 0);
    }

    public void Show()
    {
        if (!open)
        {
            open = true;
            Visible = true;
            backgroundLayer.Tweener.Stop();
            backgroundLayer.OpacityFloat = 0f;
            _ = backgroundLayer.FadeTo(0.35f, 0.35f);
            _ = this.MoveTo(0.35f, new Vector2(winSize.X + 10f, 0f), Cubic.EaseOut);
            buttonIndex = 0;
            _ = this.Schedule(0.15f, ProcessNextButton);
            scoreLabel.Visible = true;
            _ = scoreLabel.FadeIn(0.35f);
            levelLabel.Visible = true;
            _ = levelLabel.FadeIn(0.35f);
            soundButton.Visible = false;
            musicButton.Visible = false;
        }
    }

    private void OnSoundClick(TouchArguments touchArguments)
    {
        UserData.Instance.SoundDisabled = soundButton.Toggle;
        SoundManager.SoundEnabled = !soundButton.Toggle;
    }

    private void OnMusicClick(TouchArguments touchArguments)
    {
        UserData.Instance.MusicDisabled = musicButton.Toggle;
        SoundManager.MusicEnabled = !musicButton.Toggle;
    }

    private void ProcessNextButton()
    {
        ShowButton(buttons[buttonIndex]);
        buttonIndex++;
        _ = buttonIndex < buttons.Count ? this.Schedule(0.1f, ProcessNextButton) : this.Schedule(0.1f, ShowMusic);
    }

    private void ShowMusic()
    {
        ShowButton(musicButton);
        ShowButton(soundButton);
    }

    public void SetButtonsVisible(bool value)
    {
        foreach (Button button in buttons)
        {
            button.Visible = value;
        }
    }

    public static void ShowButton(Button button)
    {
        button.Scale = 0f;
        button.Visible = true;
        _ = button.FadeIn(0.3f);
        _ = button.ScaleTo(0.5f, button.RealScale, Elastic.EaseOut);
    }

    public void Hide()
    {
        if (open)
        {
            open = false;
            _ = backgroundLayer.FadeOut(0.35f);
            _ = this.MoveTo(0.5f, new Vector2(winSize.X + 300f, 0f), Cubic.EaseIn).OnComplete(OnHide);
        }
    }

    private void OnHide()
    {
        game.Paused = false;
        Visible = false;
        SetButtonsVisible(value: false);
        scoreLabel.Visible = false;
        scoreLabel.OpacityByte = 0;
        levelLabel.Visible = false;
        levelLabel.OpacityByte = 0;
    }

    private void OnPlayClick(TouchArguments touchArguments)
    {
        Hide();
    }

    private void OnRestartClick(TouchArguments touchArguments)
    {
        game.Restart();
        Hide();
    }

    private void OnMenuClick(TouchArguments touchArguments)
    {
        game.Back();
    }

    private void OnSkipClick(TouchArguments touchArguments)
    {
        game.Skip();
    }

    public void RefreshSoundButtons()
    {
        soundButton.Toggle = !SoundManager.SoundEnabled;
        musicButton.Toggle = !SoundManager.MusicEnabled;
    }
}
