using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

namespace FarseerPhysics;

public abstract class DebugViewBase
{
    protected World World { get; private set; }

    public DebugViewOptions Flags { get; set; }

    protected DebugViewBase(World world)
    {
        World = world;
    }

    public void AppendFlags(DebugViewOptions flags)
    {
        Flags |= flags;
    }

    public void RemoveFlags(DebugViewOptions flags)
    {
        Flags &= ~flags;
    }

    public abstract void DrawPolygon(Vector2[] vertices, int count, float red, float blue, float green, bool closed = true);

    public abstract void DrawSolidPolygon(Vector2[] vertices, int count, float red, float blue, float green);

    public abstract void DrawCircle(Vector2 center, float radius, float red, float blue, float green);

    public abstract void DrawSolidCircle(Vector2 center, float radius, Vector2 axis, float red, float blue, float green);

    public abstract void DrawSegment(Vector2 startPoint, Vector2 endPoint, float red, float blue, float green);

    public abstract void DrawTransform(ref Transform transform);
}
