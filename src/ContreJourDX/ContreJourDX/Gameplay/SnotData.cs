using System.Collections.Generic;
using System.Numerics;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

namespace ContreJourDX.Gameplay
{
    public class SnotData
    {
        private Vector2 localStartAnchor;

        private readonly List<Body> bodies;

        private readonly List<Joint> joints;

        public Body EyeBody { get; set; }

        public RevoluteJoint EyeJoint { get; set; }

        public Body JoinedBody { get; }

        public float InitialLength { get; }

        public Vector2 LocalStartAnchor => localStartAnchor;

        public SnotBodyClipBase Snot { get; set; }

        public RopeMetrics Metrics { get; }

        public int JoitsSize => joints.Count;

        public Body FirstBody => bodies[0];

        public Body EndBody => bodies[^1];

        public SnotData(Body eyeBody, RevoluteJoint eyeJoint, Body joinedBody, Vector2 localStartAnchor, List<Body> bodies, List<Joint> joints, RopeMetrics metrics)
        {
            EyeBody = eyeBody;
            EyeJoint = eyeJoint;
            this.bodies = bodies;
            this.joints = joints;
            JoinedBody = joinedBody;
            this.localStartAnchor = localStartAnchor;
            InitialLength = (EndBody.Position - EyeBody.Position).Length();
            Metrics = metrics;
        }

        public Body BodyAt(int i)
        {
            return bodies[i];
        }

        public Joint JointAt(int i)
        {
            return joints[i];
        }

        public int BodiesSize()
        {
            return bodies.Count;
        }

        public Vector2 GetWorldStartPoint()
        {
            return JoinedBody.GetWorldPoint(localStartAnchor);
        }
    }
}
