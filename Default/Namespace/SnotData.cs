using System.Collections.Generic;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class SnotData
{
    protected Body joinedBody;

    protected Body eyeBody;

    protected RevoluteJoint eyeJoint;

    protected Vector2 localStartAnchor;

    protected List<Body> bodies;

    protected List<Joint> joints;

    protected float initialLength;

    protected SnotBodyClipBase snot;

    protected RopeMetrics metrics;

    public Body EyeBody
    {
        get => eyeBody;
        set => eyeBody = value;
    }

    public RevoluteJoint EyeJoint
    {
        get => eyeJoint;
        set => eyeJoint = value;
    }

    public Body JoinedBody => joinedBody;

    public float InitialLength => initialLength;

    public Vector2 LocalStartAnchor => localStartAnchor;

    public SnotBodyClipBase Snot
    {
        get => snot;
        set => snot = value;
    }

    public RopeMetrics Metrics => metrics;

    public int JoitsSize => joints.Count;

    public Body FirstBody => bodies.First();

    public Body EndBody => bodies.Last();

    public SnotData(Body eyeBody, RevoluteJoint eyeJoint, Body joinedBody, Vector2 localStartAnchor, List<Body> bodies, List<Joint> joints, RopeMetrics metrics)
    {
        this.eyeBody = eyeBody;
        this.eyeJoint = eyeJoint;
        this.bodies = bodies;
        this.joints = joints;
        this.joinedBody = joinedBody;
        this.localStartAnchor = localStartAnchor;
        initialLength = (EndBody.Position - EyeBody.Position).Length();
        this.metrics = metrics;
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
        return joinedBody.GetWorldPoint(localStartAnchor);
    }
}
