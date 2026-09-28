using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace Default.Namespace;

public class ShapeProcessor : TypeProcessorBase
{
    public ShapeProcessor(string _type, LevelBuilderBase _builder)
        : base(_type, _builder)
    {
    }

    public override object ProcessItem(Hashtable item)
    {
        Hashtable hashtable = item.GetHashtable("config");
        Body val = BodyFactory.CreateBody(builder.World, (object)null);
        if (hashtable.Exists("dynamic"))
        {
            val.BodyType = (BodyType)2;
        }
        val.Position = item.GetVector("position");
        val.IsBullet = hashtable.Exists("bullet");
        if (hashtable.Exists("angularDamping"))
        {
            val.AngularDamping = hashtable.GetFloat("angularDamping");
        }
        val.FixedRotation = hashtable.GetBool("fixedRotation");
        AddShapesItem(val, item);
        if (hashtable.Exists("id"))
        {
            builder.CreatedObjects[hashtable.GetString("id")] = val;
        }
        return val;
    }

    public Fixture AddShapeItemShape(Body body, Hashtable item, Shape shape)
    {
        Hashtable hashtable = item.GetHashtable("config");
        Fixture val = body.CreateFixture(shape, (object)((!hashtable.Exists("density")) ? builder.EngineConfig.Density : hashtable.GetFloat("density")));
        val.Friction = (hashtable.Exists("friction") ? hashtable.GetFloat("friction") : builder.EngineConfig.Friction);
        val.Restitution = (hashtable.Exists("restitution") ? hashtable.GetFloat("restitution") : builder.EngineConfig.Restitution);
        val.IsSensor = hashtable.GetBool("sensor");
        if (hashtable.Exists("categoryBits") || hashtable.Exists("maskBits"))
        {
            if (hashtable.Exists("categoryBits"))
            {
                val.CollisionCategories = (Category)hashtable.GetInt("categoryBits");
            }
            if (hashtable.Exists("maskBits"))
            {
                val.CollidesWith = (Category)hashtable.GetInt("maskBits");
            }
        }
        if (hashtable.Exists("filterGroup"))
        {
            val.CollisionGroup = (short)hashtable.GetShort("filterGroup");
        }
        return val;
    }

    public virtual void AddShapesItem(Body body, Hashtable item)
    {
        AddShapeItemShape(body, item, CreateShape(item));
    }

    public virtual Shape CreateShape(Hashtable item)
    {
        return null;
    }
}
