using FarseerPhysics.Collision.Shapes;

using Microsoft.Xna.Framework;

namespace ContreJour.Gameplay;

public class CircleProcessor(LevelBuilderBase builder) : ShapeProcessor("circle", builder)
{
    public override Shape CreateShape(Hashtable item)
    {
        //IL_0029: Unknown result type (might be due to invalid IL or missing references)
        //IL_002f: Expected O, but got Unknown
        float num = item.GetFloat("radius");
        Vector2 vector = item.GetVector("position");
        CircleShape val = new(num, Builder.EngineConfig.Density)
        {
            Position = vector
        };
        return (Shape)(object)val;
    }
}
