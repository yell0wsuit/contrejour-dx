using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class LianaProcessor : JointProcessorBase
{
    private const float DENSITY = 0.3f;

    private const float RADIUS = 1f / 6f;

    public LianaProcessor(LevelBuilderBase _builder)
        : base("liana", _builder)
    {
    }

    public override object ProcessItem(Hashtable item)
    {
        return null;
    }

    public void JoinBodyTo(Body body1, Body body2)
    {
        _ = FarseerUtil.CreateDistanceJoint(builder.World, body1, body2, 4f, 0.2f);
    }

    public Body CreateBodyDynamic(Vector2 position, bool dynamic)
    {
        Body val = builder.World.CreateCircle(1f / 6f, position, 0f, 0.3f, dynamic);
        val.SetSensor(value: true);
        return val;
    }
}
