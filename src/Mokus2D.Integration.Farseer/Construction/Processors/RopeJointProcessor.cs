using System.Numerics;

using FarseerPhysics.Dynamics.Joints;

using Mokus2D.Visual;

namespace Mokus2D.Integration.Farseer.Construction.Processors
{
    public class RopeJointProcessor(PhysicsConstructor constructor) : JointProcessor(constructor)
    {
        private static readonly Vector2 EndOffset = new(10f, 0f);

        public override Joint Process(Node item)
        {
            Vector2 source = item.ZeroToGlobal();
            Vector2 target = item.LocalToGlobal(EndOffset);
            float maxLength = Constructor.ToPhysics(Vector2.Distance(source, target));
            RopeJoint ropeJoint = new(GetBodyA(item), GetBodyB(item), Vector2.Zero, Vector2.Zero)
            {
                MaxLength = maxLength
            };
            return ropeJoint;
        }
    }
}
