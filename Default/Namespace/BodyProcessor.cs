using System.Linq;

using FarseerPhysics.Dynamics;

namespace Default.Namespace;

public class BodyProcessor : ShapeProcessor
{
    protected Hashtable processors;

    public BodyProcessor(LevelBuilderBase builder)
        : base("body", builder)
    {
        processors = new Hashtable
        {
            ["circle"] = new CircleProcessor(builder),
            ["polygon"] = new PolygonProcessor(builder)
        };
    }

    public override void AddShapesItem(Body body, Hashtable item)
    {
        foreach (Hashtable array in item.GetArrayList("shapes").Cast<Hashtable>())
        {
            string key = array.GetString("config/type");
            Fixture val = AddShapeItemShape(body, item, ((ShapeProcessor)processors.GetObject(key)).CreateShape(array));
            if (array.Exists("fixtureConfig"))
            {
                val.UserData = array.GetHashtable("fixtureConfig");
            }
        }
    }
}
