using System;

using Microsoft.Xna.Framework;

using Mokus2D.Game;

namespace ContreJour.Desktop.MonoGame
{
    internal sealed class MonoGameHost : IGameHost
    {
        private readonly Game _game;

        private readonly GraphicsDeviceManager _graphics;

        public MonoGameHost(Game game, GraphicsDeviceManager graphics)
        {
            _game = game;
            _graphics = graphics;
            _game.Window.ClientSizeChanged += OnWindowClientSizeChanged;
        }

        public event Action ClientSizeChanged;

        public Mokus2D.Util.Data.Point BackBufferSize => new(_game.GraphicsDevice.PresentationParameters.BackBufferWidth, _game.GraphicsDevice.PresentationParameters.BackBufferHeight);

        public System.Numerics.Vector2 WindowSize => new(_game.GraphicsDevice.Viewport.Width, _game.GraphicsDevice.Viewport.Height);

        public Mokus2D.Util.Data.Rectangle ClientBounds
        {
            get
            {
                Rectangle bounds = _game.Window.ClientBounds;
                return new Mokus2D.Util.Data.Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            }
        }

        public bool IsActive => _game.IsActive;

        public bool IsFullScreen
        {
            get => _graphics.IsFullScreen;
            set => _graphics.IsFullScreen = value;
        }

        public Mokus2D.Util.Data.Point PreferredBackBufferSize
        {
            get => new(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            set
            {
                _graphics.PreferredBackBufferWidth = value.X;
                _graphics.PreferredBackBufferHeight = value.Y;
            }
        }

        public bool IsFixedTimeStep
        {
            get => _game.IsFixedTimeStep;
            set => _game.IsFixedTimeStep = value;
        }

        public TimeSpan TargetElapsedTime
        {
            get => _game.TargetElapsedTime;
            set => _game.TargetElapsedTime = value;
        }

        public bool IsMouseVisible
        {
            get => _game.IsMouseVisible;
            set => _game.IsMouseVisible = value;
        }

        public void ApplyGraphicsChanges()
        {
            _graphics.ApplyChanges();
        }

        public void Quit()
        {
            _game.Exit();
        }

        private void OnWindowClientSizeChanged(object sender, EventArgs e)
        {
            ClientSizeChanged?.Invoke();
        }
    }
}
