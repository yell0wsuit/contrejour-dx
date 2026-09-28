using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public static class PlatformConversions
{
    public static List<Vector2> TransformPhysicsCoords(List<Vector2> source, Vector2 physicsLevelSize)
    {
        return source;
    }

    public static Vector2 TransformLevelCoords(Vector2 source, Vector2 physicsLevelSize)
    {
        return source;
    }

    public static Vector2 VectorRelated(float x, float y)
    {
        return TransformRelatedCoords(new Vector2(x, y));
    }

    public static Vector2 TransformRelatedCoords(Vector2 source)
    {
        return source;
    }
}
