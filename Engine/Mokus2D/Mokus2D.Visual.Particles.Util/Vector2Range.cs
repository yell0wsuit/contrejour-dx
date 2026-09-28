using Default.Namespace;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;

namespace Mokus2D.Visual.Particles.Util;

public struct Vector2Range(Vector2 value, Vector2 offset)
{
    private Vector2 Value = value;

    private Vector2 Offset = offset;

    public static Vector2Range Create(Vector2 min, Vector2 max)
    {
        Vector2 vector = (min + max) / 2f;
        return new Vector2Range(vector, (max - vector).Abs());
    }

    public readonly Vector2 GetValueInRange()
    {
        return Value + (Maths.Random(-1f, 1f) * Offset);
    }
}
