using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

using ContreJour.Clips.menu;
using ContreJour.Clips.menu2;
using ContreJour.Config;
using ContreJour.Gameplay;
using ContreJour.Saving;
using ContreJour.Utils;

using Mokus2D;
using Mokus2D.Game;
using Mokus2D.Graphics;
using Mokus2D.Sound;
using Mokus2D.UI.Containers;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJour
{
    public class ContreJourApplication : Mokus2DGame
    {
        private ViewSwitcher _gameContainer;

        private LayerColor _blackForeground;

        private Node _currentView;

        private int lastLevel;

        private bool canShowIntro = true;

        private petitInformation _blockedGamePanel;

        private Vector2 _initialSize;
        private bool _restarting;

        protected virtual bool StartFullScreen => true;

        // The Windows 8 build blocked play while the app was snapped (window smaller than at launch).
        // Desktop has no snapped view, and macOS shrinks the full screen window below the notch/menu bar
        // after launch, so the original size comparison would block the game permanently.
        private static bool IsFullscreen => true;

        private static bool MultitouchSupported =>
            // The Windows 8 build refused to run without a multitouch screen. On desktop the mouse
            // is fed through the engine's cursor input instead, so don't block the game.
            true;

        public override void OnDeactivated()
        {
            base.OnDeactivated();
            // A level left while the player is away waits for them on its pause panel; menus only freeze.
            // Scenes are wrapped in a NodeContainer, so the level is its content.
            ((_currentView as NodeContainer)?.Content as ContreJourGame)?.PauseForFocusLoss();
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
            Preferences.RequestSave();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private void StartApplication()
        {
            Config.GraphicsLoader.FallbackToDefaultScaleFactor = true;
            if (ApplicationController.BackBufferSize.X >= 1200)
            {
                Config.GraphicsLoader.PrefferedScaleFactor = 0.5f;
            }
            ContentRootDirectory = "Assets/Content";
            Config.GraphicsLoader.GraphicsRootDirectory = "Graphics";
            SoundManager.MusicPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Content", "Music");
        }

        private void LoadMusic()
        {
            SoundManager.PreloadSongs(["chapter1", "chapter2", "chapter3", "chapter4", "chapter5", "menu"]);
            LoadSounds();
        }

        public override void Initialize(ApplicationController applicationController)
        {
            base.Initialize(applicationController);
            PlatformInitialize();
            Config.DefaultSpriteBatchProperties.Blend = BlendMode.AlphaBlend;
            Config.AnimationFPS = 30f;
            ApplicationController.IsFullScreen = StartFullScreen;
            ApplicationController.ApplyGraphicsChanges();
            ApplicationController.IsFixedTimeStep = false;
            StartApplication();
            LoadFonts(ContreJourLabelUtil.CultureName);
            ContreJourConfig.AspectRatio = ChooseAspectRatio();
            _gameContainer = new ViewSwitcher
            {
                ShowEffect = ShowView,
                HideEffect = HideView
            };
            _gameContainer.BeforeShowEvent += OnChangeView;
            SetRootScaleAndPosition(ApplicationController.BackBufferSize);
            applicationController.IsMouseVisible = true;
            Root.AddChild(_gameContainer);
            _blackForeground = new LayerColor(Color.Black, "menu/whitePixel");
            Root.AddChild(_blackForeground);
            _initialSize = applicationController.BackBufferSize;
            BlockGameIfNeeded();
            ShowSplash();
            applicationController.ClientSizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged()
        {
            PlatformResize();
            BlockGameIfNeeded();
        }

        public override void Update(float time)
        {
            base.Update(time);
            SoundManager.Update();
            Preferences.Update();
            PlatformUpdate();
        }

        private void HideView(Node view, Action continuation)
        {
            _blackForeground.Visible = true;
            _ = _blackForeground.FadeIn(0.5f).OnComplete((Action)delegate
            {
                OnViewHide(view, continuation);
            });
        }

        private static void OnViewHide(Node view, Action continuation)
        {
            if (view != null)
            {
                ((IDisposable)view)?.Dispose();
            }
            Preferences.RequestSave();
            GC.Collect();
            continuation();
        }

        public override void OnExiting()
        {
            base.OnExiting();
            Preferences.RequestSave();
            Preferences.Update(force: true);
        }

        private void ShowView(Node view)
        {
            _ = _blackForeground.FadeOutAndHide(1f);
        }

        private void SetRootScaleAndPosition(Mokus2D.Util.Data.Point size)
        {
            Root.Position = new Vector2(0f, ApplicationController.BackBufferSize.Y);
            Root.ScaleY = -1f;
            _gameContainer.Scale = ScreenConstants.Scales.fromIPhone2ByHeight;
            float num = size.X / ScreenConstants.OsSizes.W7.X;
            _gameContainer.Scale *= num;
            float num2 = size.X / (float)size.Y;
            Vector2 vector = size;
            if (num2 > AspectRatio.Ratio16x9.Ratio)
            {
                float num3 = size.Y * AspectRatio.Ratio16x9.Ratio;
                float num4 = (size.X - num3) / 2f;
                Root.X = num4;
                _gameContainer.Scale *= num3 / size.X;
                vector.X = num3;
                whitePixel whitePixel2 = new()
                {
                    ScaledSize = new Vector2(num4, size.Y),
                    X = 0f - num4,
                    Y = size.Y,
                    Color = Color.Black
                };
                whitePixel node = whitePixel2;
                whitePixel whitePixel3 = new()
                {
                    ScaledSize = new Vector2(num4, size.Y),
                    X = num3,
                    Y = size.Y,
                    Color = Color.Black
                };
                whitePixel node2 = whitePixel3;
                Root.AddChild(node, 1);
                Root.AddChild(node2, 1);
            }
            ContreJourConfig.RootSize = vector / _gameContainer.Scale;
        }

        private AspectRatio ChooseAspectRatio()
        {
            float num = ApplicationController.BackBufferSize.X / (float)ApplicationController.BackBufferSize.Y;
            AspectRatio[] all = AspectRatio.All;
            for (int i = 0; i < all.Length; i++)
            {
                AspectRatio result = all[i];
                if ((double)num / 1.02 < (double)result.Ratio)
                {
                    return result;
                }
            }
            return AspectRatio.All[^1];
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
            _blockedGamePanel?.VisibleAndUpdating = false;
            SoundManager.HasControl = true;
            _gameContainer.InteractionsEnabled = true;
        }

        private void ShowBlockedView()
        {
            if (_blockedGamePanel == null)
            {
                _blockedGamePanel = new petitInformation();
                Root.AddChild(_blockedGamePanel, 2);
                _blockedGamePanel.Position = _initialSize / 2f;
            }
            _blockedGamePanel.VisibleAndUpdating = true;
            _blockedGamePanel.ScaleVec = _initialSize / ApplicationController.WindowSize;
            float num = (ApplicationController.WindowSize / _blockedGamePanel.Size).Min();
            _blockedGamePanel.ScaleVec *= num;
            _blockedGamePanel.CurrentState = !MultitouchSupported ? petitInformation.State.TouchMessage : petitInformation.State.FullscreenMessage;
        }

        protected override RootNode CreateRootNode()
        {
            return new RootNode(ApplicationController.BackBufferSize, new Vector2(1f, -1f));
        }

        private void ShowSplash()
        {
            List<Action> list = [LoadMusic];
            Splash splash = new([.. list]);
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

        // Used by tools/Regression to visit every chapter's menus.
        internal void ShowMainMenu(int chapter)
        {
            ChangeScene(() => CreateMainMenu(chapter));
        }

        private MainMenu CreateMainMenu(int chapter)
        {
            MainMenu mainMenu = CreateMainMenu();
            mainMenu.ShowChapter(chapter);
            return mainMenu;
        }

        private MainMenu CreateMainMenu()
        {
            MainMenu mainMenu = new();
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

        public void LoadLevel(int level)
        {
            bool flag = IsFirstLevel(_currentView) || level == 0;
            Func<ContreJourGame> func = ProcessLoadLevel;
            if (flag)
            {
                func = CleanLoad(func);
            }
            lastLevel = level;
            ChangeScene(func);
        }

        private static Func<T> CleanLoad<T>(Func<T> action) where T : Node
        {
            ForceRemoveTextures();
            return action;
        }

        private ContreJourGame ProcessLoadLevel()
        {
            int chapter = LevelsMenu.GetLevelPosition(lastLevel).Chapter;
            ContreJourGame contreJourGame = new(chapter)
            {
                CanShowIntro = canShowIntro
            };
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

        public static void ForceRemoveTextures()
        {
        }

        public static bool IsFirstLevel(Node node)
        {
            return node is ContreJourGame game && game.LevelIndex == 0;
        }

        private static void LoadSounds()
        {
            SoundManager.PreloadSounds(
            [
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
                "deathByFall1",
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
            ]);
        }

        public override void OnApplicationViewChanged(EventArgs args)
        {
            base.OnApplicationViewChanged(args);
            BlockGameIfNeeded();
        }

        private static void PlatformUpdate()
        {
        }

        public static void PlatformInitialize()
        {
        }

        private static void PlatformResize()
        {
        }

    }
}
