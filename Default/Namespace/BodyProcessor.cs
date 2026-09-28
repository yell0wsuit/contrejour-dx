using FarseerPhysics.Dynamics;

namespace Default.Namespace;

public class BodyProcessor : ShapeProcessor
{
    protected Hashtable processors;

    public BodyProcessor(LevelBuilderBase _builder)
        : base("body", _builder)
    {
        processors = new Hashtable();
        processors["circle"] = new CircleProcessor(_builder);
        processors["polygon"] = new PolygonProcessor(_builder);
    }

    public override void AddShapesItem(Body body, Hashtable item)
    {
        foreach (Hashtable array in item.GetArrayList("shapes"))
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
