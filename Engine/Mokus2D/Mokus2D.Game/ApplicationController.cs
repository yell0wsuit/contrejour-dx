using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Mokus2D.Content;
using Mokus2D.Platforms.Input;
using Mokus2D.Util.Data;
using Mokus2D.Util.Resources;

namespace Mokus2D.Game;

public class ApplicationController : DisposableBase
{
	protected readonly Mokus2DGame Game;

	private readonly GraphicsDeviceManager _graphics;

	public readonly Microsoft.Xna.Framework.Game Application;

	public MokusContentManager Content => (MokusContentManager)Application.Content;

	public Vector2 WindowSize => new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

	public bool IsFullScreen
	{
		get
		{
			return _graphics.IsFullScreen;
		}
		set
		{
			_graphics.IsFullScreen = value;
		}
	}

	public Rectangle ClientBounds => Application.Window.ClientBounds;

	public bool IsFixedTimeStep
	{
		get
		{
			return Application.IsFixedTimeStep;
		}
		set
		{
			Application.IsFixedTimeStep = value;
		}
	}

	public Mokus2D.Util.Data.Point PrefferedBackBufferSize
	{
		get
		{
			return new Mokus2D.Util.Data.Point(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
		}
		set
		{
			_graphics.PreferredBackBufferWidth = value.X;
			_graphics.PreferredBackBufferHeight = value.Y;
		}
	}

	public Mokus2D.Util.Data.Point BackBufferSize => new Mokus2D.Util.Data.Point(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);

	public bool SynchronizeWithVerticalRetrace
	{
		get
		{
			return _graphics.SynchronizeWithVerticalRetrace;
		}
		set
		{
			_graphics.SynchronizeWithVerticalRetrace = value;
		}
	}

	public bool IsMouseVisible
	{
		get
		{
			return Application.IsMouseVisible;
		}
		set
		{
			Application.IsMouseVisible = value;
		}
	}

	public string ContentRootDirectory
	{
		get
		{
			return Application.Content.RootDirectory;
		}
		set
		{
			Application.Content.RootDirectory = value;
		}
	}

	public bool IsActive => Application.IsActive;

	public TimeSpan TargetElapsedTime
	{
		get
		{
			return Application.TargetElapsedTime;
		}
		set
		{
			Application.TargetElapsedTime = value;
		}
	}

	public GraphicsDevice GraphicsDevice => Application.GraphicsDevice;

	public ApplicationController(Mokus2DGame game, Microsoft.Xna.Framework.Game application, GraphicsDeviceManager graphics)
	{
		Game = game;
		Application = application;
		_graphics = graphics;
	}

	public void OnInitialize()
	{
		Mouse.WindowHandle = Application.Window.Handle;
		TouchPanel.WindowHandle = Application.Window.Handle;
		Application.GraphicsDevice.RasterizerState = new RasterizerState
		{
			CullMode = CullMode.None
		};
		Application.GraphicsDevice.DepthStencilState = DepthStencilState.None;
	}

	public void ApplyGraphicsChanges()
	{
		_graphics.ApplyChanges();
	}

	public void Update(float time)
	{
		MouseController.Update(time);
	}

	public void Exit()
	{
	}
}
