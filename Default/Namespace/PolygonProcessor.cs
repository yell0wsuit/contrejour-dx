using FarseerPhysics.Collision.Shapes;

using Mokus2D.Util;

namespace Default.Namespace;

public class PolygonProcessor(LevelBuilderBase _builder) : ShapeProcessor("polygon", _builder)
{
    public override Shape CreateShape(Hashtable item)
    {
        //IL_0012: Unknown result type (might be due to invalid IL or missing references)
        //IL_0018: Expected O, but got Unknown
        //IL_001e: Unknown result type (might be due to invalid IL or missing references)
        //IL_0024: Expected O, but got Unknown
        return new PolygonShape([.. item.GetArrayList("vertices").ToVectorList()], 0.3f);
    }
}
