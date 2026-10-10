using System.Numerics;

using FarseerPhysics.Dynamics;

namespace ContreJourDX.Gameplay
{
    public class LianaProcessor(LevelBuilderBase builder) : JointProcessorBase("liana", builder)
    {
        public override object ProcessItem(Hashtable item)
        {
            return null;
        }

        public void JoinBodyTo(Body body1, Body body2)
        {
            _ = FarseerUtil.CreateDistanceJoint(Builder.World, body1, body2, 4f, 0.2f);
        }

        public Body CreateBodyDynamic(Vector2 position, bool dynamic)
        {
            Body val = Builder.World.CreateCircle(1f / 6f, position, 0f, 0.3f, dynamic);
            val.SetSensor(value: true);
            return val;
        }
    }
}
