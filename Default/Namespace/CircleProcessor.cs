using FarseerPhysics.Collision.Shapes;
using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class CircleProcessor : ShapeProcessor
{
    public CircleProcessor(LevelBuilderBase _builder)
        : base("circle", _builder)
    {
    }

    public override Shape CreateShape(Hashtable item)
    {
        //IL_0029: Unknown result type (might be due to invalid IL or missing references)
        //IL_002f: Expected O, but got Unknown
        float num = item.GetFloat("radius");
        Vector2 vector = item.GetVector("position");
        CircleShape val = new CircleShape(num, builder.EngineConfig.Density);
        val.Position = vector;
        return (Shape)(object)val;
    }
}
