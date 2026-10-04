using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

using ContreJour.Clips;
using ContreJour.Config;
using ContreJour.Gameplay.Interfaces;
using ContreJour.Utils;

using Mokus2D;
using Mokus2D.Events;
using Mokus2D.Graphics;
using Mokus2D.Input;
using Mokus2D.Sound;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;
using Mokus2D.Visual.Text;
using Mokus2D.Visual.Util;

namespace ContreJour.Gameplay
{
    public class MainMenu : AccelerometerMenu, IActivatedDependent
    {
        private LevelsMenu levelsMenu;

        public EventSender<int> LevelSelectEvent { get; } = new();

        public EventSender ExitEvent { get; } = new();

        private Sprite ground;

        private Sprite logo;

        private List<Sprite> backgroundImages;

        private LayerColor blackLayer;

        private Label starsField;

        private Sprite starsIcon;

        private Vector2 centerPosition;

        private Node background;

        private readonly Button backButton;

        private ToggleButton soundButton;

        private ToggleButton musicButton;

        private readonly ClickableLayer clickableLayer;

        private NamesChanger names;

        private readonly Node foreground;

        private PlanetsSpinner spinner;

        private BackgroundChanger backgroundChanger;

        private bool inChapter;

        private bool inLevel;

        private int currentChapter;

        private Vector2 winSize;
        private static int loadedLevel = -1;

        private static readonly Color BlueColor = ContreJourConstants.BlueLightColor;

        private static readonly Color GreyColor = ContreJourConstants.GreyColor;

        private static readonly Color GreenColor = 12573952.ToRGBColor();

        private static readonly Color[] FontColors =
        [
            GreyColor,
            Color.Lerp(Color.White, BlueColor, 0.8f),
            ContreJourConditions.Trial(Color.Lerp(Color.White, BlueColor, 0.8f), GreyColor),
            GreyColor,
            GreyColor,
            ContreJourConstants.NewFriendColor,
            GreenColor
        ];

        private static readonly Color[] BackColors = Constants.IsTrial ? [GreyColor, BlueColor, BlueColor] :
        [
            GreyColor,
            BlueColor,
            ContreJourConditions.Trial(BlueColor, GreyColor),
            ColorUtil.Mult(GreyColor, 0.5f),
            GreyColor,
            ContreJourConstants.NewFriendColor,
            GreenColor
        ];

        private Vector2 namesPosition;

        private readonly float RADIUS = ContreJourConditions.Trial(340, 360);

        private readonly float scoreY = 20f;

        public bool PlanetsLoaded { get; private set; }

        public MainMenu()
        {
            SoundManager.MusicDisableEvent += OnMusicDisable;
            foreground = new Node();
            AddChild(foreground, 3);
            winSize = ContreJourConfig.RootSize;
            centerPosition = ContreJourConfig.RootSize / 2f;
            CreateBackgrounds();
            CreatePlanets();
            CreateLabels();
            CreateScore();
            clickableLayer = new ClickableLayer(-1);
            AddChild(clickableLayer, 5);
            if (ContreJourConfig.BackButtonVisible)
            {
                backButton = new Button("menu/McBackIcon")
                {
                    Position = ContreJourConfig.BackButtonPosition,
                    RealScale = 1.2f,
                    Visible = false
                };
                clickableLayer.AddChild(backButton);
                backButton.TouchEndEvent += delegate
                {
                    OnBackClick();
                };
                backButton.Enabled = false;
            }
            if (loadedLevel != -1 && !spinner.Exploding)
            {
                LevelPosition levelPosition = LevelsMenu.GetLevelPosition(loadedLevel);
                spinner.Visible = false;
                currentChapter = levelPosition.MenuChapter;
                spinner.CurrentIndex = currentChapter;
                names.Visible = false;
                names.OpacityByte = 0;
                if (ContreJourConfig.BackButtonVisible)
                {
                    backButton.Visible = true;
                    backButton.Enabled = true;
                }
                CreateLevelsMenu();
                RefreshScore();
                inChapter = true;
                spinner.Enabled = false;
                FixCurrentPosition();
            }
            else
            {
                PlanetsLoaded = true;
                SoundManager.PlayMusic("menu");
            }
            CreateButtons();
            CreateLiteButtons();
            _ = Constants.IsTrial;
            AddKeyListeners();
        }

        private void AddKeyListeners()
        {
            KeysController keys = Mokus2DGame.Instance.KeysController;
            keys.AddBackKeyListener(OnBackKey);
            keys.AddKeyListener(Key.Left, OnLeftKey);
            keys.AddKeyListener(Key.Right, OnRightKey);
            keys.AddKeyListener(Key.Enter, OnEnterKey);
        }

        private void RemoveKeyListeners()
        {
            KeysController keys = Mokus2DGame.Instance.KeysController;
            keys.RemoveBackKeyListener(OnBackKey);
            keys.RemoveKeyListener(Key.Left, OnLeftKey);
            keys.RemoveKeyListener(Key.Right, OnRightKey);
            keys.RemoveKeyListener(Key.Enter, OnEnterKey);
        }

        // Esc does what the back button does, whenever the button could be pressed.
        private void OnBackKey()
        {
            if (!inLevel && backButton?.Enabled == true)
            {
                OnBackClick();
            }
        }

        private void OnLeftKey()
        {
            if (!inChapter)
            {
                spinner.Step(-1);
            }
        }

        private void OnRightKey()
        {
            if (!inChapter)
            {
                spinner.Step(1);
            }
        }

        private void OnEnterKey()
        {
            if (!inChapter)
            {
                spinner.ClickCentered();
            }
        }

        private void OnMusicDisable()
        {
            musicButton.Toggle = true;
        }

        public void AddForeground(Node node)
        {
            foreground.AddChild(node);
        }

        public static void CreateLiteButtons()
        {
        }

        public override bool TouchBegin(Touch touch)
        {
            return base.TouchBegin(touch);
        }

        public void CreateButtons()
        {
            musicButton = new ToggleButton("menu/McButtonMenuBackground", "menu/McMusicIcon", "menu/McDisabledIcon");
            ApplyButtonProperties(musicButton);
            musicButton.Position = new Vector2(winSize.X - 76f, 0f);
            musicButton.TouchEndEvent += OnMusicClick;
            musicButton.ToggleIcon.IgnoreParentColor = true;
            soundButton = new ToggleButton("menu/McButtonMenuBackground", "menu/McSoundIcon", "menu/McDisabledIcon");
            ApplyButtonProperties(soundButton);
            soundButton.ToggleIcon.IgnoreParentColor = true;
            soundButton.Position = musicButton.Position - new Vector2(soundButton.TextureSize.X * soundButton.RealScale, 0f);
            soundButton.TouchEndEvent += OnSoundClick;
            RefreshSoundButtons();
        }

        private void ApplyButtonProperties(Button button)
        {
            button.StopEventPropagation = true;
            button.AnchorY = 1f;
            button.RealScale = 0.765f;
            foreach (Node child in button.Children)
            {
                child.Y = button.TextureSize.Y / 2f;
                child.Scale = 2f;
            }
            clickableLayer.AddChild(button);
        }

        private void RefreshSoundButtons()
        {
            soundButton.Toggle = !SoundManager.SoundEnabled;
            musicButton.Toggle = !SoundManager.MusicEnabled;
        }

        public static void CacheImages()
        {
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

        public void CreatePlanets()
        {
            spinner = new PlanetsSpinner(this);
            AddChild(spinner);
            spinner.Position = centerPosition;
            spinner.SelectEvent.AddListener(OnChapterSelect);
            spinner.PlanetsScale = winSize.Y / 660f;
        }

        public void CreateLabels()
        {
            names = new NamesChanger(spinner.PlanetsScale.Clamp(0.8f, 1.2f));
            namesPosition = new Vector2(190f, winSize.Y - 136f);
            names.Position = namesPosition;
            AddChild(names, 4);
            logo = new Sprite(ClipIds.Menu.McMainMenuLogo);
            AddChild(logo, 4);
            logo.Position = new Vector2(winSize.X - 80f - logo.Size.X, winSize.Y);
            if (ContreJourConfig.BackButtonVisible)
            {
                logo.X -= 70f;
            }
            logo.Scale = 1.2f;
        }

        public void CreateBackgrounds()
        {
            List<string> list = Backgrounds();
            backgroundImages = [];
            background = new Node();
            AddChild(background, -2);
            background.Scale = 1.3f;
            int num = 1;
            foreach (string item in list)
            {
                Sprite sprite = new(item)
                {
                    Scale = 2.2f,
                    Position = new Vector2(0f, winSize.Y + 20f)
                };
                sprite.Scale *= 0.9375f;
                sprite.OpacityFloat = 0f;
                sprite.Visible = false;
                background.AddChild(sprite);
                backgroundImages.Add(sprite);
                num++;
            }
            blackLayer = new LayerColor(new Color(0, 0, 0, 255), "menu/whitePixel");
            AddChild(blackLayer, 2);
            blackLayer.OpacityByte = 0;
            blackLayer.Visible = false;
            ground = new Sprite(ClipIds.Menu.McMenuGroundPhone);
            Sprite mcMenuGroundPhone = new(ClipIds.Menu.McMenuGroundPhone);
            ground.AddChild(mcMenuGroundPhone);
            mcMenuGroundPhone.Position = new Vector2(ground.Size.X - 2f, winSize.Y + 4f);
            mcMenuGroundPhone.RotationDegrees = 180f;
            ground.ScaleX = (winSize.X + 8f) / ground.Size.X;
            ground.Position = new Vector2(-2f, -2f);
            AddChild(ground, 3);
            backgroundImages[1].Color = BlueColor;
            // Rekindled lowers the Mango menu background; Windows 8 never shipped it.
            backgroundImages[Constants.BonusChapter].Position = new Vector2(0f, winSize.Y - 100f);
            if (Constants.IsTrial)
            {
                backgroundImages[2].Color = BlueColor;
            }
            backgroundChanger = new BackgroundChanger(backgroundImages);
        }

        public void CreateScore()
        {
            starsIcon = new Sprite(ClipIds.Menu.McEnergyIcon);
            AddChild(starsIcon, 4);
            starsIcon.Position = new Vector2(20f, scoreY);
            starsIcon.Scale = 1.3f;
            starsField = ContreJourLabelUtil.CreateLabel(16f);
            starsField.Anchor = new Vector2(0f, 0.5f);
            AddChild(starsField, 4);
            starsField.Position = new Vector2(48f, scoreY - 2f);
            starsField.Visible = !Constants.IsTrial;
            starsIcon.Visible = starsField.Visible;
            RefreshScore();
        }

        public static List<string> Backgrounds()
        {
            return
            [
                "menuBackgrounds/McBackground4Content",
                "menuBackgrounds/McBackgroundContent1_5",
                "menu/McMenuBackground3",
                "menu2/McChapter4MenuBackground",
                "menu2/McChapter5MenuBackground",
                "newFriend/McChapter5MenuBackground",
                "chapter6/McBackgroundContent6_1",
            ];
        }

        internal void OnChapterSelect(int chapter)
        {
            if (Constants.IsTrial && chapter == Constants.ChaptersCount)
            {
                OnGetFullVersion();
            }
            else if (!inChapter && spinner.Scale == 1f)
            {
                SoundManager.PlayRandomSound(Sounds.Tap, 0.5f);
                currentChapter = chapter;
                HidePlanets();
            }
        }

        public void HideScore()
        {
            _ = starsField.FadeOut(0.3f);
            _ = starsIcon.FadeOut(0.3f);
        }

        public void ShowScore()
        {
            RefreshScore();
            _ = starsField.FadeIn(0.3f);
            _ = starsIcon.FadeIn(0.3f);
        }

        public void RefreshScore()
        {
            int num = inChapter ? UserData.Instance.GetChapterStars(currentChapter) : UserData.Instance.TotalStars;
            int num2 = inChapter ? UserData.Instance.GetChapterScore(currentChapter) : UserData.Instance.TotalScore;
            int num3 = inChapter ? LevelsMenu.GetLevelCount(currentChapter) * 3 : (ContreJourConstants.LevelCount * 3);
            string textString = string.Format(CultureInfo.CurrentCulture, Messages.StarsAndScoreFormat, num, num3, num2);
            starsField.TextString = textString;
        }

        public void HidePlanets()
        {
            HideScore();
            blackLayer.Tweener.Stop();
            inChapter = true;
            spinner.Enabled = false;
            _ = spinner.ScaleTo(0.3f, 5f).OnComplete(ShowLevels);
            _ = names.FadeOut(0.1f).OnComplete(NodeValues.Hide);
            blackLayer.Visible = true;
            _ = blackLayer.FadeIn(0.3f);
        }

        public void ShowLevels()
        {
            ShowScore();
            spinner.Visible = false;
            if (ContreJourConfig.BackButtonVisible)
            {
                _ = this.Schedule(0.3f, OnLevelsShow);
            }
            _ = blackLayer.FadeOut(2f).OnComplete(NodeValues.Hide);
            CreateLevelsMenu();
            levelsMenu.Scale = levelsMenu.InitialScale * 0.5f;
            _ = levelsMenu.ScaleTo(0.3f, levelsMenu.InitialScale);
            levelsMenu.Position = winSize / 2f;
            levelsMenu.Show();
        }

        public void CreateLevelsMenu()
        {
            levelsMenu = new LevelsMenu(currentChapter, winSize / 2f);
            levelsMenu.GetMoreEvent.AddListener(OnGetFullVersion);
            levelsMenu.SelectLevelEvent += OnLevelSelect;
            levelsMenu.InitialScale *= winSize.Y / 650f;
            levelsMenu.Scale = levelsMenu.InitialScale;
            AddChild(levelsMenu);
        }

        private void OnLevelSelect(int level)
        {
            if (LevelSelectEvent.Enabled)
            {
                inLevel = true;
                loadedLevel = level;
                LevelSelectEvent.SendEvent(level);
                LevelSelectEvent.Enabled = false;
            }
        }

        private void OnLevelsShow()
        {
            backButton.Visible = true;
            backButton.OpacityByte = 0;
            _ = backButton.FadeIn(0.3f);
            _ = this.Schedule(0.3f, EnableBack);
        }

        private void EnableBack()
        {
            backButton.Enabled = true;
        }

        private void OnBackClick()
        {
            if (inChapter)
            {
                HideLevels();
                inChapter = false;
            }
            else
            {
                Mokus2DGame.Instance.Exit();
            }
        }

        public void HideLevels()
        {
            HideScore();
            blackLayer.Tweener.Stop();
            if (ContreJourConfig.BackButtonVisible)
            {
                _ = backButton.FadeOut(0.3f);
                backButton.Enabled = false;
            }
            _ = blackLayer.FadeIn(0.3f).OnComplete(ShowPlanets);
            _ = (levelsMenu?.ScaleTo(0.3f, 0.5f));
            blackLayer.Visible = true;
        }

        private void ShowPlanets()
        {
            PlanetsLoaded = true;
            SoundManager.PlayMusic("menu");
            names.Tweener.Stop();
            names.Visible = true;
            _ = names.FadeTo(0.3f, 1f);
            AccelerometerUsed = false;
            inChapter = false;
            ShowScore();
            if (ContreJourConfig.BackButtonVisible)
            {
                backButton.Visible = false;
            }
            if (levelsMenu != null)
            {
                RemoveChild(levelsMenu);
            }
            levelsMenu = null;
            _ = blackLayer.FadeOutAndHide(1f);
            spinner.Visible = true;
            _ = spinner.ScaleTo(0.3f, 1f);
            spinner.Enabled = true;
        }

        public void ShowChapter(int chapter)
        {
            spinner.Scale = 1f;
            if (inChapter)
            {
                levelsMenu.Scale = 0f;
                levelsMenu.Visible = false;
                HideLevels();
            }
            spinner.SetTargetChapter(chapter);
        }

        private static void FixCurrentPosition()
        {
        }

        public override void Update(float time)
        {
            base.Update(time);
            RefreshPosition();
            RefreshColors();
        }

        private void RefreshColors()
        {
            Color color = Color.Lerp(BackColors[backgroundChanger.NextIndex], BackColors[backgroundChanger.FirstIndex], 1f - backgroundChanger.Offset);
            Color color2 = Color.Lerp(Color.White, color, 0.7f);
            ground.Color = color;
            logo.Color = color2;
            Color color3 = Color.Lerp(FontColors[backgroundChanger.NextIndex], FontColors[backgroundChanger.FirstIndex], 1f - backgroundChanger.Offset);
            starsField.Color = color3;
            soundButton.Color = color2;
            musicButton.Color = color2;
            if (ContreJourConfig.BackButtonVisible)
            {
                backButton.Color = color2;
            }
        }

        public static void OnGetFullVersion()
        {
            SoundManager.PlayRandomSound(Sounds.Tap, 0.7f);
        }

        public void RefreshPosition()
        {
            backgroundChanger.CurrentIndex = spinner.CurrentIndex;
            names.CurrentIndex = spinner.CurrentIndex;
            if (!inChapter)
            {
                spinner.AccelerometerOffset = AccelerometerOffset;
            }
            else if (!inLevel && levelsMenu != null)
            {
                levelsMenu.Position = new Vector2(AccelerometerOffset.X * 1.05f * RADIUS / 2f, AccelerometerOffset.Y * 0.25f) + (winSize / 2f);
            }
            AccelerometerUsed = true;
            Vector2 vector = new((0f - AccelerometerOffset.X) * 1.05f * RADIUS, (0f - AccelerometerOffset.Y) * 0.1f);
            background.Position = vector + new Vector2(-60f, -50f);
            Vector2 vector2 = new(AccelerometerOffset.X * 0.3f * RADIUS, AccelerometerOffset.Y * 0.07f);
            names.Position = namesPosition + vector2;
            foreground.Position = vector2 * 0.2f;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            RemoveKeyListeners();
            ExitEvent.RemoveListeners();
            LevelSelectEvent.RemoveListeners();
            spinner.Dispose();
        }

        public void OnGameActivated()
        {
            RefreshSoundButtons();
        }
    }
}
