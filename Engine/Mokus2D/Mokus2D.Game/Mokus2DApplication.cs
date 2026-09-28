using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mokus2D.Content;
using Mokus2D.OSInteraction;

namespace Mokus2D.Game;

public class Mokus2DApplication<T> : Microsoft.Xna.Framework.Game where T : Mokus2DGame, new()
{
	private readonly GraphicsDeviceManager _graphics;

	private readonly T _game;

	public Mokus2DApplication()
	{
		WindowsUtil.Initialize(base.Window);
		_game = new T();
		_graphics = new GraphicsDeviceManager(this);
		_graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
		_graphics.ApplyChanges();
	}

	protected override void Initialize()
	{
		base.Initialize();
		InitializePlatform();
		base.Content = new MokusContentManager(base.Services);
		ApplicationController applicationController = new ApplicationController(_game, this, _graphics);
		T game = _game;
		game.Initialize(applicationController);
	}

	protected override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
		T game = _game;
		game.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
	}

	protected override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
		base.GraphicsDevice.Clear(_game.BackgroundColor);
		T game = _game;
		game.Draw();
	}

	protected override void OnActivated(object sender, EventArgs args)
	{
		base.OnActivated(sender, args);
		T game = _game;
		game.OnActivated();
	}

	protected override void OnDeactivated(object sender, EventArgs args)
	{
		base.OnDeactivated(sender, args);
		T game = _game;
		game.OnDeactivated();
	}

	protected override void OnExiting(object sender, EventArgs args)
	{
		base.OnExiting(sender, args);
		T game = _game;
		game.OnExiting();
	}

	private void InitializePlatform()
	{
		base.ApplicationViewChanged += OnApplicationViewChanged;
	}

	private void OnApplicationViewChanged(object sender, ViewStateChangedEventArgs e)
	{
		T game = _game;
		game.OnApplicationViewChanged(e);
	}
}
