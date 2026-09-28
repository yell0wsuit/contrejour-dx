using System.Collections.Generic;

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;

using Microsoft.Xna.Framework;

using Mokus2D.Util;

namespace Default.Namespace;

public class PolygonProcessor : ShapeProcessor
{
    public PolygonProcessor(LevelBuilderBase _builder)
        : base("polygon", _builder)
    {
    }

    public override Shape CreateShape(Hashtable item)
    {
        //IL_0012: Unknown result type (might be due to invalid IL or missing references)
        //IL_0018: Expected O, but got Unknown
        //IL_001e: Unknown result type (might be due to invalid IL or missing references)
        //IL_0024: Expected O, but got Unknown
        return (Shape)new PolygonShape(new Vertices((IEnumerable<Vector2>)item.GetArrayList("vertices").ToVectorList()), 0.3f);
    }
}
