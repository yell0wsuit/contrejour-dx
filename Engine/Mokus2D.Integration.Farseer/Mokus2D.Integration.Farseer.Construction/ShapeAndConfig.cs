using System.Collections.Generic;

using FarseerPhysics.Collision.Shapes;

namespace Mokus2D.Integration.Farseer.Construction;

public struct ShapeAndConfig(Shape shape, IDictionary<string, string> config)
{
    public readonly Shape Shape = shape;

    public readonly IDictionary<string, string> Config = config;
}
