using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Content;

namespace Mokus2D.Game;

public class Mokus2DApplication<T> : Microsoft.Xna.Framework.Game where T : Mokus2DGame, new()
{
    private readonly GraphicsDeviceManager _graphics;

    private readonly T _game;

    public Mokus2DApplication()
    {
        _game = new T();
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferFormat = SurfaceFormat.Color,
            // Windows 8 apps always ran full screen at native resolution; mirror that with borderless full screen.
            HardwareModeSwitch = false
        };
        DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
        _graphics.PreferredBackBufferWidth = displayMode.Width;
        _graphics.PreferredBackBufferHeight = displayMode.Height;
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        base.Initialize();
        Content = new MokusContentManager(Services);
        ApplicationController applicationController = new(this, _graphics);
        T game = _game;
        game.Initialize(applicationController);
        // Subscribe after the game is set up: resizes applied during Initialize would otherwise
        // reach the game before its views exist (Win8 view-state events only arrived later).
        InitializePlatform();
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
        GraphicsDevice.Clear(_game.BackgroundColor);
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

    protected override void OnExiting(object sender, ExitingEventArgs args)
    {
        base.OnExiting(sender, args);
        T game = _game;
        game.OnExiting();
    }

    private void InitializePlatform()
    {
        Window.ClientSizeChanged += OnApplicationViewChanged;
    }

    private void OnApplicationViewChanged(object sender, EventArgs e)
    {
        T game = _game;
        game.OnApplicationViewChanged(e);
    }
}
