using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

using Mokus2D;
using Mokus2D.FileSystem;
using Mokus2D.Game;
using Mokus2D.Sound;

namespace ContreJour.Desktop.MonoGame
{
    public class MonoGameApplication<T> : Game where T : Mokus2DGame, new()
    {
        private readonly GraphicsDeviceManager _graphics;

        private readonly T _game;

        private readonly IAudioBackend _audio;

        private ApplicationController _applicationController;

        public MonoGameApplication(IAudioBackend audio)
        {
            _audio = audio;
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
            MonoGameHost host = new(this, _graphics);
            Mouse.WindowHandle = Window.Handle;
            TouchPanel.WindowHandle = Window.Handle;
            GraphicsDevice.RasterizerState = new RasterizerState
            {
                CullMode = CullMode.None
            };
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            MonoGameInputSource input = new();
            _applicationController = new ApplicationController(host, input, new FileLoader(), _audio, GraphicsDevice);
            _game.Initialize(_applicationController);
            // Subscribe after the game is set up: resizes applied during Initialize would otherwise
            // reach the game before its views exist (Win8 view-state events only arrived later).
            host.ClientSizeChanged += OnApplicationViewChanged;
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _game.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.Clear(_game.BackgroundColor);
            _game.Draw();
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
            base.OnActivated(sender, args);
            _game.OnActivated();
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            base.OnDeactivated(sender, args);
            _game.OnDeactivated();
        }

        protected override void OnExiting(object sender, ExitingEventArgs args)
        {
            base.OnExiting(sender, args);
            _game.OnExiting();
        }

        protected override void Dispose(bool disposing)
        {
            // Release the cached textures while the graphics device is still alive.
            if (disposing)
            {
                _applicationController?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void OnApplicationViewChanged()
        {
            _game.OnApplicationViewChanged(EventArgs.Empty);
        }
    }
}
