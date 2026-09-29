using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Content;
using Mokus2D.FileSystem;
using Mokus2D.Input;
using Mokus2D.Platforms.Input;
using Mokus2D.Util;
using Mokus2D.Util.Resources;

namespace Mokus2D.Game
{
    // The engine's view of the platform host. GraphicsDevice and MokusContentManager are still
    // MonoGame types: rendering and content loading do not go through platform interfaces yet.
    public class ApplicationController : DisposableBase
    {
        public ApplicationController(IGameHost host, IInputSource input, IFileLoader files, GraphicsDevice graphicsDevice, MokusContentManager content)
        {
            Host = host;
            Input = input;
            Files = files;
            GraphicsDevice = graphicsDevice;
            Content = content;
            Host.ClientSizeChanged += OnHostClientSizeChanged;
        }

        public IGameHost Host { get; }

        public IInputSource Input { get; }

        public IFileLoader Files { get; }

        public MokusContentManager Content { get; }

        public GraphicsDevice GraphicsDevice { get; }

        public event Action ClientSizeChanged;

        public Vector2 WindowSize => Host.WindowSize;

        public bool IsFullScreen
        {
            get => Host.IsFullScreen;
            set => Host.IsFullScreen = value;
        }

        public Rectangle ClientBounds => Host.ClientBounds;

        public bool IsFixedTimeStep
        {
            get => Host.IsFixedTimeStep;
            set => Host.IsFixedTimeStep = value;
        }

        public Util.Data.Point PrefferedBackBufferSize
        {
            get => Host.PreferredBackBufferSize;
            set => Host.PreferredBackBufferSize = value;
        }

        public Util.Data.Point BackBufferSize => Host.BackBufferSize;

        public bool IsMouseVisible
        {
            get => Host.IsMouseVisible;
            set => Host.IsMouseVisible = value;
        }

        public string ContentRootDirectory
        {
            get => Content.RootDirectory;
            set => Content.RootDirectory = value;
        }

        public bool IsActive => Host.IsActive;

        public TimeSpan TargetElapsedTime
        {
            get => Host.TargetElapsedTime;
            set => Host.TargetElapsedTime = value;
        }

        public void ApplyGraphicsChanges()
        {
            Host.ApplyGraphicsChanges();
        }

        public static void Update()
        {
            MouseController.Update();
        }

        // The Windows 8 game could not close itself, so the quit path through Mokus2DGame.Exit does nothing.
        public static void Exit()
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Host.ClientSizeChanged -= OnHostClientSizeChanged;
            }
            base.Dispose(disposing);
        }

        private void OnHostClientSizeChanged()
        {
            ClientSizeChanged.Dispatch();
        }
    }
}
