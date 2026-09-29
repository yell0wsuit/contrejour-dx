using System;

namespace FarseerPhysics;

[Flags]
public enum DebugViewOptions
{
    Shape = 1,
    Joint = 2,
    AABB = 4,
    CenterOfMass = 0x10,
    DebugPanel = 0x20,
    ContactPoints = 0x40,
    ContactNormals = 0x80,
    PolygonPoints = 0x100,
    PerformanceGraph = 0x200,
    Controllers = 0x400
}
