using System.Numerics;

namespace Mokus2D.Input
{
    public readonly record struct MouseSnapshot(Vector2 Position, bool Left, bool Middle, bool Right, int ScrollWheelValue);
}
