using System;
using System.Numerics;

using Mokus2D.Game;
using Mokus2D.Util.Data;

namespace ContreJour.Regression
{
    // A host with no window: always active, never resized by anyone but the game, and done when the
    // game quits.
    internal sealed class HeadlessHost : IGameHost
    {
        private bool _sizeLocked;

        public HeadlessHost()
        {
            string size = Environment.GetEnvironmentVariable("CJ_REGRESSION_SIZE");
            if (!string.IsNullOrEmpty(size))
            {
                string[] dimensions = size.Split('x');
                Point bufferSize = new(int.Parse(dimensions[0], System.Globalization.CultureInfo.InvariantCulture), int.Parse(dimensions[1], System.Globalization.CultureInfo.InvariantCulture));
                BackBufferSize = PreferredBackBufferSize = bufferSize;
            }
        }

        // Nothing outside the game ever resizes a headless back buffer.
        public event Action ClientSizeChanged
        {
            add { }
            remove { }
        }

        public Point BackBufferSize { get; private set; } = new(1280, 720);

        public Vector2 WindowSize => new(BackBufferSize.X, BackBufferSize.Y);

        public Rectangle ClientBounds => new(0, 0, BackBufferSize.X, BackBufferSize.Y);

        public bool IsActive => true;

        public bool IsFullScreen { get; set; }

        public Point PreferredBackBufferSize { get; set; } = new(1280, 720);

        public bool IsFixedTimeStep { get; set; } = true;

        public TimeSpan TargetElapsedTime { get; set; } = TimeSpan.FromTicks(166667L);

        public bool IsMouseVisible { get; set; }

        public bool QuitRequested { get; private set; }

        public void ApplyGraphicsChanges()
        {
            if (PreferredBackBufferSize == BackBufferSize)
            {
                return;
            }
            if (_sizeLocked)
            {
                throw new NotSupportedException("The headless back buffer cannot be resized once drawing has started.");
            }
            BackBufferSize = PreferredBackBufferSize;
        }

        // Called once the drawing surface exists at BackBufferSize.
        public void LockSize()
        {
            _sizeLocked = true;
        }

        public void Quit()
        {
            QuitRequested = true;
        }
    }
}
