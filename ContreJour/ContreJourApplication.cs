using System;
using System.Collections.Generic;
using System.IO;

using ContreJour.Clips.level1;
using ContreJour.Clips.menu;
using ContreJour.Clips.menu2;
using ContreJour.Clips.segoeFont;
using ContreJour.Config;
using ContreJour.WinRT;

using Default.Namespace;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

using Mokus2D;
using Mokus2D.Config.Tint;
using Mokus2D.Fonts;
using Mokus2D.Game;
using Mokus2D.Sound;
using Mokus2D.UI.Containers;
using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Text;

namespace ContreJour;

public class ContreJourApplication : Mokus2DGame
{
    private const float FADE_OUT_DURATION = 0.5f;

    private const float FADE_IN_DURATION = 1f;

    private const int StripesLayer = 1;

    private const int BlockedViewLayer = 2;

    private ViewStack _gameContainer;

    private LayerColor _blackForeground;

    private Node _currentView;

    protected int lastLevel;

    private bool canShowIntro = true;

    private petitInformation _blockedGamePanel;

    private Vector2 _initialSize;

    private static readonly Dictionary<int, FontData> fonts = new Dictionary<int, FontData>();

    private Label _upsLabel;

    private bool _restarting;

    public static Dictionary<int, FontData> Fonts => fonts;

    protected virtual bool StartFullScreen => true;

    // The Windows 8 build blocked play while the app was snapped (window smaller than at launch).
    // Desktop has no snapped view, and macOS shrinks the full screen window below the notch/menu bar
    // after launch, so the original size comparison would block the game permanently.
    private bool IsFullscreen => true;

    private bool MultitouchSupported
    {
        get
        {
            // The Windows 8 build refused to run without a multitouch screen. On desktop the mouse
            // is fed through the engine's cursor input instead, so don't block the game.
            return true;
        }
    }

    public override void OnResumeComplete()
    {
        base.OnResumeComplete();
        SoundManager.OnResume();
        UserData.Instance.RefreshSoundManager();
        if (_restarting)
        {
            _restarting = false;
            ShowSplash();
        }
    }

    private void OnChangeView(Node node)
    {
        UserData.SaveUserData();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }

    private void StartApplication()
    {
        Mokus2DGame.Config.GraphicsLoader.FallbackToDefaultScaleFactor = true;
        if (base.ApplicationController.BackBufferSize.X >= 1200)
        {
            Mokus2DGame.Config.GraphicsLoader.PrefferedScaleFactor = 0.5f;
        }
        base.ContentRootDirectory = "Assets/Content";
        Mokus2DGame.Config.GraphicsLoader.GraphicsRootDirectory = "Graphics";
        SoundManager.MusicPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Content", "Music");
    }

    private void LoadMusic()
    {
        SoundManager.PreloadSongs(new string[6] { "chapter1", "chapter2", "chapter3", "chapter4", "chapter5", "menu" });
        LoadSounds();
    }

    public override void Initialize(ApplicationController applicationController)
    {
        base.Initialize(applicationController);
        PlatformInitialize();
        Mokus2DGame.Config.RenderTargetEnabled = false;
        Mokus2DGame.Config.DefaultSpriteBatchProperties.Blend = BlendState.AlphaBlend;
        Mokus2DGame.Config.AnimationFPS = 30f;
        TintGraphicsConfig graphicsConfig = new TintGraphicsConfig(tintEnabled: false);
        Mokus2DGame.Config.GraphicsConfig = graphicsConfig;
        base.ApplicationController.IsFullScreen = StartFullScreen;
        base.ApplicationController.ApplyGraphicsChanges();
        base.ApplicationController.IsFixedTimeStep = false;
        StartApplication();
        SegoePrint28Label.Register();
        ContreJourConfig.AspectRatio = ChooseAspectRatio();
        _gameContainer = new ViewStack();
        _gameContainer.ShowEffect = ShowView;
        _gameContainer.HideEffect = HideView;
        _gameContainer.BeforeShowEvent += OnChangeView;
        SetRootScaleAndPosition(base.ApplicationController.BackBufferSize);
        applicationController.IsMouseVisible = true;
        base.Root.AddChild(_gameContainer);
        _blackForeground = new LayerColor(Color.Black, "menu/whitePixel");
        base.Root.AddChild(_blackForeground);
        _initialSize = applicationController.BackBufferSize;
        BlockGameIfNeeded();
        ShowSplash();
        applicationController.Application.Window.ClientSizeChanged += OnSizeChanged;
        UserData.Instance.TotalStarsChanged += OnTotalStarsChanged;
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        PlatformResize();
        BlockGameIfNeeded();
    }

    public override void Update(float time)
    {
        base.Update(time);
        SoundManager.Update();
        PlatformUpdate();
    }

    private void OnTotalStarsChanged(int stars)
    {
        LiveTileUpdater.UpdateTiles(stars);
    }

    private void HideView(Node view, Action continuation)
    {
        _blackForeground.Visible = true;
        _blackForeground.FadeIn(0.5f).OnComplete((Action)delegate
        {
            OnViewHide(view, continuation);
        });
    }

    private void OnViewHide(Node view, Action continuation)
    {
        if (view != null)
        {
            ((IDisposable)view)?.Dispose();
        }
        UserData.SaveUserData();
        GC.Collect();
        continuation();
    }

    public override void OnExiting()
    {
        base.OnExiting();
        UserData.SaveUserData();
    }

    private void ShowView(Node view)
    {
        _blackForeground.FadeOutAndHide(1f);
    }

    private void Test()
    {
        McPuddle mcPuddle = new McPuddle();
        mcPuddle.XY = 200f;
        mcPuddle.CurrentFrame = mcPuddle.TotalFrames - 1;
        base.Root.AddChild(mcPuddle);
    }

    private void SetRootScaleAndPosition(Mokus2D.Util.Data.Point size)
    {
        base.Root.Position = new Vector2(0f, base.ApplicationController.BackBufferSize.Y);
        base.Root.ScaleY = -1f;
        _gameContainer.Scale = ScreenConstants.Scales.fromIPhone2ByHeight;
        float num = (float)size.X / ScreenConstants.OsSizes.W7.X;
        _gameContainer.Scale *= num;
        float num2 = (float)size.X / (float)size.Y;
        Vector2 vector = size;
        if (num2 > AspectRatio.Ratio16x9.Ratio)
        {
            float num3 = (float)size.Y * AspectRatio.Ratio16x9.Ratio;
            float num4 = ((float)size.X - num3) / 2f;
            base.Root.X = num4;
            _gameContainer.Scale *= num3 / (float)size.X;
            vector.X = num3;
            whitePixel whitePixel2 = new whitePixel();
            whitePixel2.ScaledSize = new Vector2(num4, size.Y);
            whitePixel2.X = 0f - num4;
            whitePixel2.Y = size.Y;
            whitePixel2.Color = Color.Black;
            whitePixel node = whitePixel2;
            whitePixel whitePixel3 = new whitePixel();
            whitePixel3.ScaledSize = new Vector2(num4, size.Y);
            whitePixel3.X = num3;
            whitePixel3.Y = size.Y;
            whitePixel3.Color = Color.Black;
            whitePixel node2 = whitePixel3;
            base.Root.AddChild(node, 1);
            base.Root.AddChild(node2, 1);
        }
        ContreJourConfig.RootSize = vector / _gameContainer.Scale;
    }

    private AspectRatio ChooseAspectRatio()
    {
        float num = (float)base.ApplicationController.BackBufferSize.X / (float)base.ApplicationController.BackBufferSize.Y;
        AspectRatio[] all = AspectRatio.All;
        for (int i = 0; i < all.Length; i++)
        {
            AspectRatio result = all[i];
            if ((double)num / 1.02 < (double)result.Ratio)
            {
                return result;
            }
        }
        return AspectRatio.All.Last();
    }

    private void BlockGameIfNeeded()
    {
        if (!IsFullscreen || !MultitouchSupported)
        {
            _gameContainer.VisibleAndUpdating = false;
            ShowBlockedView();
            SoundManager.HasControl = false;
            _gameContainer.InteractionsEnabled = false;
            return;
        }
        _gameContainer.VisibleAndUpdating = true;
        if (_blockedGamePanel != null)
        {
            _blockedGamePanel.VisibleAndUpdating = false;
        }
        SoundManager.HasControl = true;
        _gameContainer.InteractionsEnabled = true;
    }

    private void ShowBlockedView()
    {
        if (_blockedGamePanel == null)
        {
            _blockedGamePanel = new petitInformation();
            base.Root.AddChild(_blockedGamePanel, 2);
            _blockedGamePanel.Position = _initialSize / 2f;
        }
        _blockedGamePanel.VisibleAndUpdating = true;
        _blockedGamePanel.ScaleVec = _initialSize / base.ApplicationController.WindowSize;
        float num = (base.ApplicationController.WindowSize / _blockedGamePanel.Size).Min();
        _blockedGamePanel.ScaleVec *= num;
        if (!MultitouchSupported)
        {
            _blockedGamePanel.CurrentState = petitInformation.State.TouchMessage;
        }
        else
        {
            _blockedGamePanel.CurrentState = petitInformation.State.FullscreenMessage;
        }
    }

    protected override RootNode CreateRootNode()
    {
        return new RootNode(base.ApplicationController.BackBufferSize, new Vector2(1f, -1f));
    }

    private void ShowSplash()
    {
        List<Action> list = new List<Action>();
        list.Add(LoadMusic);
        Splash splash = new Splash(list.ToArray());
        _gameContainer.CurrentView = splash;
        splash.EndEvent.AddListener(OnSplashExit);
        _currentView = splash;
    }

    private void OnSplashExit()
    {
        if (!UserData.Instance.IntroWatched)
        {
            UserData.Instance.IntroWatched = true;
            LoadLevel(0);
        }
        else
        {
            ChangeScene(CreateMainMenu);
        }
    }

    private void ChangeScene<T>(Func<T> sceneFactory) where T : Node
    {
        _gameContainer.CurrentView = SetCurrentNode(sceneFactory);
    }

    private Node SetCurrentNode<T>(Func<T> nodeFactory) where T : Node
    {
        _currentView = new NodeContainer(nodeFactory);
        return _currentView;
    }

    private MainMenu CreateMainMenu(int chapter)
    {
        MainMenu mainMenu = CreateMainMenu();
        mainMenu.ShowChapter(chapter);
        return mainMenu;
    }

    private MainMenu CreateMainMenu()
    {
        MainMenu mainMenu = new MainMenu();
        mainMenu.LevelSelectEvent.AddListener(LoadLevel);
        mainMenu.ExitEvent.AddListener(OnMainMenuExit);
        return mainMenu;
    }

    private void OnMainMenuExit()
    {
        _currentView.Dispose();
        _gameContainer.RemoveAllChildren();
        GC.Collect();
        _restarting = true;
        SoundManager.StopMusic();
    }

    private Splash CreateSplash()
    {
        Splash splash = new Splash();
        splash.EndEvent.AddListener(OnSplashExit);
        return splash;
    }

    public void LoadLevel(int _level)
    {
        bool flag = IsFirstLevel(_currentView) || _level == 0;
        Func<ContreJourGame> func = ProcessLoadLevel;
        if (flag)
        {
            func = CleanLoad(func);
        }
        lastLevel = _level;
        ChangeScene(func);
    }

    private Func<T> CleanLoad<T>(Func<T> action) where T : Node
    {
        ForceRemoveTextures();
        return action;
    }

    private ContreJourGame ProcessLoadLevel()
    {
        int chapter = LevelsMenu.GetLevelPosition(lastLevel).Chapter;
        ContreJourGame contreJourGame = new ContreJourGame(chapter);
        contreJourGame.CanShowIntro = canShowIntro;
        contreJourGame.BackEvent.AddListener(OnLevelBack);
        contreJourGame.RestartEvent.AddListener(RestartLevel);
        contreJourGame.NextLevelEvent.AddListener(NextLevel);
        contreJourGame.LoadLevelIndex(lastLevel);
        SoundManager.PlayMusic($"chapter{chapter + 1}");
        return contreJourGame;
    }

    private void OnLevelBack()
    {
        canShowIntro = true;
        Func<MainMenu> sceneFactory = CreateMainMenu;
        if (IsFirstLevel(_currentView))
        {
            sceneFactory = CleanLoad(CreateMainMenu);
        }
        ChangeScene(sceneFactory);
    }

    private void RestartLevel()
    {
        canShowIntro = false;
        ChangeScene(ProcessLoadLevel);
    }

    public void NextLevel()
    {
        canShowIntro = true;
        LevelPosition levelPosition = LevelsMenu.GetLevelPosition(lastLevel);
        if (levelPosition.Index < Constants.LevelsToPlay - 1)
        {
            levelPosition.Index++;
            LoadLevel(LevelsMenu.GetLevelIndex(levelPosition));
        }
        else if (levelPosition.Chapter == 5)
        {
            ChangeScene(() => CreateMainMenu(5));
        }
        else if (ContreJourConditions.Trial(trialValue: true, levelPosition.Chapter + 1 < Constants.NormalChaptersCount))
        {
            int chapter = levelPosition.Chapter + 1;
            ChangeScene(() => CreateMainMenu(chapter));
        }
        else
        {
            LoadLevel(169);
        }
    }

    public void ForceRemoveTextures()
    {
    }

    public bool IsFirstLevel(Node node)
    {
        if (node is ContreJourGame)
        {
            return ((ContreJourGame)node).LevelIndex == 0;
        }
        return false;
    }

    private static void LoadSounds()
    {
        SoundManager.PreloadSounds(new string[45]
        {
            "angry2",
            "backgroundEyeHit0",
            "begin5",
            "bell",
            "bonus5",
            "bonus6",
            "bonus7",
            "boom0",
            "breathIn4",
            "click",
            "clip0",
            "clip1",
            "deathByFall2",
            "deathByFlowerOut10",
            "deathByFlowerOut4",
            "deathBySpikes5",
            "end",
            "explosion0",
            "explosion1",
            "fly",
            "landing1",
            "landing3",
            "laugh0",
            "laugh1",
            "laughl3",
            "leapOn1",
            "leapOn2",
            "leapOn3",
            "newClip",
            "newClip1",
            "perdelkaOut0",
            "perdelkaOutEmpty1",
            "petitkoIsHoping",
            "petitkoIsTrying",
            "rope2",
            "rope3",
            "rope5",
            "saddness",
            "sleeping0",
            "spring",
            "suspicious0",
            "suspicious1",
            "suspicious3",
            "teleport",
            Sounds.IntroSound
        });
    }

    public override void OnApplicationViewChanged(EventArgs args)
    {
        base.OnApplicationViewChanged(args);
        BlockGameIfNeeded();
    }

    private void OnResizeToFullscreen()
    {
    }

    private void PlatformUpdate()
    {
    }

    public void PlatformInitialize()
    {
    }

    private void PlatformResize()
    {
    }
}
