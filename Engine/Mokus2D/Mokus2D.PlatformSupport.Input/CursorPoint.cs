using Microsoft.Xna.Framework;

using Mokus2D.Input;

namespace Mokus2D.PlatformSupport.Input;

public struct CursorPoint(Vector2 position, int id, TouchType type)
{
    public readonly Vector2 Position = position;

    public readonly int Id = id;

    public readonly TouchType Type = type;
}
