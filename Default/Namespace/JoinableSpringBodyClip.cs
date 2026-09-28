using FarseerPhysics.Dynamics.Joints;

using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Default.Namespace;

public class JoinableSpringBodyClip : RotatableSpringBase
{
    private const float QUERY_RADIUS = 1.6666666f;

    protected RotatorBodyClip rotator;

    protected Vector2 relativeRotatorPosition;

    protected float relativeAngle;

    protected float stickedAngle;

    protected RevoluteJoint rotatorJoint;

    protected override bool IsMoving
    {
        get
        {
            if (rotator != null)
            {
                return rotator.Body.AngularVelocity != 0f;
            }
            return false;
        }
    }

    public JoinableSpringBodyClip(LevelBuilderBase _builder, object _body, Node _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
        rotator = (RotatorBodyClip)FarseerUtil.Query(builder.World, Body.Position, 1.6666666f, typeof(RotatorBodyClip));
        relativeRotatorPosition = rotator.Body.GetLocalPoint(Body.Position);
        relativeAngle = rotator.Body.Rotation - Body.Rotation;
    }

    private void DestroyRotatorJoint()
    {
        if (rotatorJoint != null)
        {
            builder.World.RemoveJoint((Joint)(object)rotatorJoint);
            rotatorJoint = null;
        }
    }

    private void FixPosition()
    {
        Vector2 worldPoint = rotator.Body.GetWorldPoint(relativeRotatorPosition);
        float num = rotator.Body.Rotation - relativeAngle;
        Body.SetTransform(worldPoint, num);
    }

    private void CreateRotatorJoint()
    {
        if (rotatorJoint == null)
        {
            rotatorJoint = FarseerUtil.CreateRevoluteJoint(builder.World, Body, rotator.Body, Body.Position, collideConnected: false, limitAngles: true);
        }
    }

    public override void Update(float time)
    {
        if (rotator != null)
        {
            FixPosition();
        }
        base.Update(time);
    }

    protected override void CreateShadow()
    {
    }
}
