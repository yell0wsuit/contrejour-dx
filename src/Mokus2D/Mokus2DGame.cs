using System;
using System.IO;
using System.Numerics;

using Mokus2D.Config;
using Mokus2D.Content;
using Mokus2D.Effects.Tweening;
using Mokus2D.FileSystem;
using Mokus2D.Game;
using Mokus2D.Graphics;
using Mokus2D.Input;
using Mokus2D.Util;
using Mokus2D.Util.Data;
using Mokus2D.Util.Resources;
using Mokus2D.Util.Schedule;
using Mokus2D.Visual;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Interfaces;
using Mokus2D.Visual.Text;

namespace Mokus2D
{
    public abstract class Mokus2DGame : DisposableBase
    {
        public KeysController KeysController { get; } = new();

        internal Scheduler Scheduler { get; } = new();

        public TouchController TouchController { get; } = new();

        public SpriteClicksListener SpriteClicksListener { get; }

        private readonly Tweener Tweener;

        public UpdateDrawCounter PerformanceCounter { get; } = new(60);

        private GameConfig _config;

        private readonly FontsManager _fontsManager = new();

        private FontRegistry _fonts;

        private BatchSelector _batchSelector;

        private readonly KeyboardController _keyboard = new();

        public Color BackgroundColor { get; set; } = Color.Black;

        private readonly float? MaxUpdateTime = 0.04f;

        private readonly ConcurrentDelayedActions _mainThreadActions = new();

        public static Mokus2DGame Instance { get; private set; }

        protected ApplicationController ApplicationController { get; private set; }

        public bool IsExiting { get; private set; }

        public Vector2 ScreenSize => ApplicationController.BackBufferSize;

        public static BatchSelector BatchSelector
        {
            get
            {
                if (Instance._batchSelector == null)
                {
                    Instance._batchSelector = new BatchSelector();
                }
                return Instance._batchSelector;
            }
        }

        public static IFileLoader FileLoader => Instance.ApplicationController.Files;

        public static FontsManager FontsManager => Instance._fontsManager;

        public static FontRegistry Fonts => Instance._fonts ?? throw new InvalidOperationException("LoadFonts must be called before labels are created.");

        public static KeyboardController Keyboard => Instance._keyboard;

        public static IInputSource Input => Instance.ApplicationController.Input;

        public static GameConfig Config => Instance._config;

        public static IRenderer Renderer => Instance.ApplicationController.Renderer;

        public static Vector2 ScreenCenter => Instance.ScreenSize * 0.5f;

        public static MokusContentManager ContentManager => Instance.ApplicationController.Content;

        public Vector2 WindowSize => ApplicationController.WindowSize;

        public bool IsActive => ApplicationController.IsActive;

        // Lets automated runs (tools/Regression) ignore the real mouse, keyboard and touch input.
        public bool InputEnabled { get; set; } = true;

        public bool AcceptsInput => InputEnabled && IsActive;

        public bool IsFullScreen
        {
            get => ApplicationController.IsFullScreen;
            set => ApplicationController.IsFullScreen = value;
        }

        public Point PrefferedBackBufferSize
        {
            get => ApplicationController.PrefferedBackBufferSize;
            set => ApplicationController.PrefferedBackBufferSize = value;
        }

        public RootNode Root
        {
            get
            {
                field ??= CreateRootNode();
                return field;
            }
        }

        public string ContentRootDirectory
        {
            set => ApplicationController.ContentRootDirectory = value;
        }

        public Rectangle ClientBounds => ApplicationController.ClientBounds;

        public TimeSpan TargetElapsedTime => ApplicationController.TargetElapsedTime;

        public event Action Exiting;

        public virtual void OnApplicationViewChanged(EventArgs args)
        {
        }

        public static ISpriteData LoadSpriteData(string name)
        {
            return Config.GraphicsLoader.Load<ISpriteData>(name);
        }

        public static IMovieClipData LoadMovieClipData(string name)
        {
            return Config.GraphicsLoader.Load<IMovieClipData>(name);
        }

        public static AnimationData LoadAnimation(string name)
        {
            return Config.GraphicsLoader.Load<AnimationData>(name);
        }

        public static T LoadResource<T>(string name)
        {
            return Config.GraphicsLoader.Load<T>(name);
        }

        public static void RegisterFont(string fontName, string fontId)
        {
            FontsManager.RegisterFont(fontName, fontId);
        }

        // Loads the face for a two-letter locale from <content root>/fonts/fonts.json, replacing any earlier one.
        public static void LoadFonts(string locale)
        {
            FontRegistry fonts = FontRegistry.Load(FileLoader, Path.Combine(ContentManager.RootDirectory, "fonts"), locale, Renderer.CreateFontFace);
            Instance._fonts?.Dispose();
            Instance._fonts = fonts;
        }

        public static void RunInMainThread(Action action)
        {
            Instance.DoRunInMainThread(action);
        }

        public static void ResetScaleFactor(float value)
        {
            if (Config.GraphicsLoader.PrefferedScaleFactor != value)
            {
                Config.GraphicsLoader.PrefferedScaleFactor = value;
                FontsManager.ReloadFonts();
            }
        }

        protected Mokus2DGame()
        {
            Instance = this;
            SpriteClicksListener = new SpriteClicksListener();
            Tweener = new Tweener(this);
        }

        public static Vector2 ScreenPosition(Vector2 anchor)
        {
            return Instance.ScreenSize * anchor;
        }

        private void DoRunInMainThread(Action action)
        {
            _mainThreadActions.Add(action);
        }

        public virtual void OnActivated()
        {
        }

        public virtual void OnDeactivated()
        {
            KeyboardController.OnGameDeactivated();
        }

        public virtual void OnResume()
        {
        }

        public virtual void OnResumeComplete()
        {
        }

        public void Exit()
        {
            IsExiting = true;
            ApplicationController.Exit();
        }

        public virtual void OnExiting()
        {
            IsExiting = true;
            KeyboardController.OnGameExit();
            Exiting.Dispatch();
        }

        public void ApplyGraphicsChanges()
        {
            ApplicationController.ApplyGraphicsChanges();
        }

        protected virtual RootNode CreateRootNode()
        {
            return new RootNode(ApplicationController.BackBufferSize);
        }

        public virtual void Update(float time)
        {
            PerformanceCounter.StartUpdate();
            RunMainThreadActions();
            if (MaxUpdateTime.HasValue)
            {
                time = Math.Min(time, MaxUpdateTime.Value);
            }
            if (InputEnabled)
            {
                KeysController.Update(time);
            }
            if (AcceptsInput)
            {
                TouchController.Update(time);
            }
            Scheduler.Update(time);
            ApplicationController.Update();
            Tweener.Update(time);
            Root.UpdateNode(time);
            PerformanceCounter.EndUpdate();
            PerformanceCounter.Update(time);
        }

        private void RunMainThreadActions()
        {
            _mainThreadActions.Execute();
        }

        public virtual void Initialize(ApplicationController applicationController)
        {
            ApplicationController = applicationController;
            ApplicationController.IsMouseVisible = true;
            ContentRootDirectory = "Content";
            ApplicationController.TargetElapsedTime = TimeSpan.FromTicks(166667L);
            _config = new GameConfig();
            Config.GraphicsLoader.GraphicsRootDirectory = "Graphics";
        }

        public virtual void Draw()
        {
            PerformanceCounter.StartDraw();
            Root.DrawAll();
            PerformanceCounter.EndDraw();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ApplicationController?.Dispose();
        }
    }
}
