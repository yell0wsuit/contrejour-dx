using System;

using Microsoft.Xna.Framework;

namespace Mokus2D.Game
{
    /// <summary>The platform window and main loop the engine runs in.</summary>
    public interface IGameHost
    {
        Util.Data.Point BackBufferSize { get; }

        /// <summary>Size of the current viewport.</summary>
        Vector2 WindowSize { get; }

        Rectangle ClientBounds { get; }

        bool IsActive { get; }

        bool IsFullScreen { get; set; }

        Util.Data.Point PreferredBackBufferSize { get; set; }

        bool IsFixedTimeStep { get; set; }

        TimeSpan TargetElapsedTime { get; set; }

        bool IsMouseVisible { get; set; }

        event Action ClientSizeChanged;

        void ApplyGraphicsChanges();

        void Quit();
    }
}
