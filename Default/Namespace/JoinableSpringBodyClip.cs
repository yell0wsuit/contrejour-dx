using FarseerPhysics.Dynamics.Joints;

using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Default.Namespace;

public class JoinableSpringBodyClip : RotatableSpringBase
{
    private RotatorBodyClip rotator;

    private Vector2 relativeRotatorPosition;

    private float relativeAngle;

    private float stickedAngle;

    private RevoluteJoint rotatorJoint;

    protected override bool IsMoving => rotator != null && rotator.Body.AngularVelocity != 0f;

    public JoinableSpringBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        rotator = (RotatorBodyClip)FarseerUtil.Query(this.builder.World, Body.Position, 1.6666666f, typeof(RotatorBodyClip));
        relativeRotatorPosition = rotator.Body.GetLocalPoint(Body.Position);
        relativeAngle = rotator.Body.Rotation - Body.Rotation;
    }

    private void FixPosition()
    {
        Vector2 worldPoint = rotator.Body.GetWorldPoint(relativeRotatorPosition);
        float num = rotator.Body.Rotation - relativeAngle;
        Body.SetTransform(worldPoint, num);
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
