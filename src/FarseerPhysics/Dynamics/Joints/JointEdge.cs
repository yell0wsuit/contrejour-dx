namespace FarseerPhysics.Dynamics.Joints
{
    public sealed class JointEdge
    {
        public Joint Joint { get; set; }

        public JointEdge Next { get; set; }

        public Body Other { get; set; }

        public JointEdge Prev { get; set; }
    }
}
