using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

using Mokus2D.Content;
using Mokus2D.Platforms.Input;
using Mokus2D.Util.Resources;

namespace Mokus2D.Game
{
    public class ApplicationController(Microsoft.Xna.Framework.Game application, GraphicsDeviceManager graphics) : DisposableBase
    {

        private readonly GraphicsDeviceManager _graphics = graphics;

        public Microsoft.Xna.Framework.Game Application { get; } = application;

        public MokusContentManager Content => (MokusContentManager)Application.Content;

        public Vector2 WindowSize => new(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

        public bool IsFullScreen
        {
            get => _graphics.IsFullScreen;
            set => _graphics.IsFullScreen = value;
        }

        public Rectangle ClientBounds => Application.Window.ClientBounds;

        public bool IsFixedTimeStep
        {
            get => Application.IsFixedTimeStep;
            set => Application.IsFixedTimeStep = value;
        }

        public Util.Data.Point PrefferedBackBufferSize
        {
            get => new(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            set
            {
                _graphics.PreferredBackBufferWidth = value.X;
                _graphics.PreferredBackBufferHeight = value.Y;
            }
        }

        public Util.Data.Point BackBufferSize => new(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);

        public bool SynchronizeWithVerticalRetrace
        {
            get => _graphics.SynchronizeWithVerticalRetrace;
            set => _graphics.SynchronizeWithVerticalRetrace = value;
        }

        public bool IsMouseVisible
        {
            get => Application.IsMouseVisible;
            set => Application.IsMouseVisible = value;
        }

        public string ContentRootDirectory
        {
            get => Application.Content.RootDirectory;
            set => Application.Content.RootDirectory = value;
        }

        public bool IsActive => Application.IsActive;

        public TimeSpan TargetElapsedTime
        {
            get => Application.TargetElapsedTime;
            set => Application.TargetElapsedTime = value;
        }

        public GraphicsDevice GraphicsDevice => Application.GraphicsDevice;

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

        public static void Update()
        {
            MouseController.Update();
        }

        public static void Exit()
        {
        }
    }
}
