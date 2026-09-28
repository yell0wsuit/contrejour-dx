using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mokus2D.Config;
using Mokus2D.Content;
using Mokus2D.Effects.Tweening;
using Mokus2D.FileSystem;
using Mokus2D.Game;
using Mokus2D.Input;
using Mokus2D.Util;
using Mokus2D.Util.Data;
using Mokus2D.Util.Resources;
using Mokus2D.Util.Schedule;
using Mokus2D.Visual;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.GameDebug;
using Mokus2D.Visual.Interfaces;
using Mokus2D.Visual.Text;
using Mokus2D.Visual.Util;

namespace Mokus2D;

public abstract class Mokus2DGame : DisposableBase
{
	public readonly KeysController KeysController = new KeysController();

	public readonly Scheduler Scheduler = new Scheduler();

	public readonly TouchController TouchController = new TouchController();

	public readonly SpriteClicksListener SpriteClicksListener;

	public Tweener Tweener;

	private readonly GarbageTracer _schedulerGarbageTracer = new GarbageTracer("Scheduler", start: false);

	private readonly GarbageTracer _touchGarbageTracer = new GarbageTracer("TouchController", start: false);

	public readonly UpdateDrawCounter PerformanceCounter = new UpdateDrawCounter(60);

	private GameConfig _config;

	private readonly FontsManager _fontsManager = new FontsManager();

	private BatchSelector _batchSelector;

	private readonly KeyboardController _keyboard = new KeyboardController();

	public Color BackgroundColor = Color.Black;

	public float? MaxUpdateTime = 0.04f;

	private RootNode _root;

	private GarbageTracer _gameDrawGarbageTracer = new GarbageTracer("Game.Draw", start: false);

	private GarbageTracer _gameUpdateGarbageTracer = new GarbageTracer("Game.Update", start: false);

	private GarbageTracer _rootGarbageTracer = new GarbageTracer("Root.Update", start: false);

	private IFileLoader _fileLoader = new FileLoader();

	private readonly ConcurrentDelayedActions _mainThreadActions = new ConcurrentDelayedActions();

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

	public static IFileLoader FileLoader
	{
		get
		{
			return Instance._fileLoader;
		}
		set
		{
			Instance._fileLoader = value;
		}
	}

	public static FontsManager FontsManager => Instance._fontsManager;

	public static KeyboardController Keyboard => Instance._keyboard;

	public static GameConfig Config => Instance._config;

	public static GraphicsDevice Device => Instance.ApplicationController.GraphicsDevice;

	public static Vector2 ScreenCenter => Instance.ScreenSize * 0.5f;

	public static MokusContentManager ContentManager => Instance.ApplicationController.Content;

	public Vector2 WindowSize => ApplicationController.WindowSize;

	public bool IsActive => ApplicationController.IsActive;

	// Lets automated runs (tools/Regression) ignore the real mouse, keyboard and touch input.
	public bool InputEnabled { get; set; } = true;

	public bool AcceptsInput => InputEnabled && IsActive;

	public bool IsFullScreen
	{
		get
		{
			return ApplicationController.IsFullScreen;
		}
		set
		{
			ApplicationController.IsFullScreen = value;
		}
	}

	public Mokus2D.Util.Data.Point PrefferedBackBufferSize
	{
		get
		{
			return ApplicationController.PrefferedBackBufferSize;
		}
		set
		{
			ApplicationController.PrefferedBackBufferSize = value;
		}
	}

	public RootNode Root
	{
		get
		{
			if (_root == null)
			{
				_root = CreateRootNode();
			}
			return _root;
		}
	}

	public string ContentRootDirectory
	{
		set
		{
			ApplicationController.ContentRootDirectory = value;
		}
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
		PrimitivesDrawing.Clear();
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
		Keyboard.OnGameDeactivated();
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
		Keyboard.OnGameExit();
		this.Exiting.Dispatch();
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
		ApplicationController.Update(time);
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
		ApplicationController.OnInitialize();
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
		if (ApplicationController != null)
		{
			ApplicationController.Dispose();
		}
	}
}
