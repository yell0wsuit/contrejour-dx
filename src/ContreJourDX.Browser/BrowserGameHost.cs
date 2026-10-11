using System;
using System.Numerics;

using Mokus2D.Game;
using Mokus2D.Util.Data;

namespace ContreJourDX.Browser
{
    // The page as the engine sees it. As on desktop, the back buffer is a fixed logical canvas chosen once (the
    // screen's size at Play): the game lays itself out once, so later page sizes only move the letterbox.
    internal sealed class BrowserGameHost : IGameHost
    {
        public BrowserGameHost(Point canvas, Vector2 cssSize, Vector2 pixelSize)
        {
            BackBufferSize = canvas;
            PreferredBackBufferSize = canvas;
            Resize(cssSize, pixelSize);
        }

        // The canvas never changes size, so this never fires.
        public event Action ClientSizeChanged
        {
            add { }
            remove { }
        }

        public Point BackBufferSize { get; }

        public Vector2 WindowSize => new(BackBufferSize.X, BackBufferSize.Y);

        public Rectangle ClientBounds => new(0, 0, BackBufferSize.X, BackBufferSize.Y);

        public bool IsActive { get; set; } = true;

        // The browser build has no full screen of its own; the game's request at startup is ignored. The
        // browser's own full screen (F11) only resizes the page, which the letterbox follows.
        public bool IsFullScreen
        {
            get => false;
            set { }
        }

        public Point PreferredBackBufferSize { get; set; }

        // The loop always steps variable time; these are kept for the engine to read back.
        public bool IsFixedTimeStep { get; set; }

        public TimeSpan TargetElapsedTime { get; set; }

        public bool IsMouseVisible
        {
            get;
            set
            {
                field = value;
                PageInterop.SetCursorVisible(value);
            }
        } = true;

        public Letterbox Letterbox { get; private set; }

        public void ApplyGraphicsChanges()
        {
            if (PreferredBackBufferSize != BackBufferSize)
            {
                throw new NotSupportedException("The browser back buffer is fixed at Play; the page is letterboxed instead.");
            }
        }

        // A page cannot close itself, and Mokus2DGame.Exit never asks.
        public void Quit()
        {
        }

        public void Resize(Vector2 cssSize, Vector2 pixelSize)
        {
            Letterbox = new Letterbox(new Vector2(BackBufferSize.X, BackBufferSize.Y), cssSize, pixelSize);
        }
    }
}
