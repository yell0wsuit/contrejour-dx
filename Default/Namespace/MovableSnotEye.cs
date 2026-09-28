using System;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

using Microsoft.Xna.Framework;

using Mokus2D.Input;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace Default.Namespace;

public class MovableSnotEye : SnotEye, IRestartable
{
    private bool moving;

    private Touch movingTouch;

    private SnotPoint targetPoint;

    private bool restoreJoint;

    private float targetSpeed;

    private readonly SnotPoint initialPoint;

    private Vector2 targetPosition;

    public MovableSnotEye(SnotBodyClip _snot, Body _body, SnotPoint targetPoint)
        : base(_snot, _body)
    {
        initialPoint = targetPoint;
        this.targetPoint = targetPoint;
    }

    public override int Priority(Vector2 touchPosition)
    {
        return !snot.Joined ? 0 : 1;
    }

    protected override void CheckTouchDistance(Touch touch, float distance)
    {
        if (distance > 55f * builder.SizeMult && snot.Linked == null)
        {
            hasRelease = false;
            moving = true;
            movingTouch = touch;
            snot.Enabled = false;
            DestroyEyeJoint();
            snot.Physics.EyeJoint = null;
        }
        else
        {
            base.CheckTouchDistance(touch, distance);
        }
    }

    private void DestroyEyeJoint()
    {
        if (snot.Physics.EyeJoint != null)
        {
            restoreJoint = true;
            builder.World.RemoveJoint((Joint)(object)snot.Physics.EyeJoint);
        }
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (moving)
        {
            DragSnot();
        }
        else if (!moving && targetPoint != null && !Body.Position.FuzzyEquals(targetPoint.Body.Position))
        {
            MoveTo(Body.Position.StepTo(targetPoint.Body.Position, targetSpeed * time));
        }
        else if (!snot.Enabled)
        {
            EndSnotDrag();
        }
        targetPoint?.Enabled = snot.Linked == null;
    }

    private void EndSnotDrag()
    {
        Body.BodyType = (BodyType)2;
        snot.Enabled = true;
        if (restoreJoint)
        {
            snot.Physics.EyeJoint = FarseerUtil.CreateRevoluteJoint(builder.World, snot.Physics.JoinedBody, snot.Physics.EyeBody, targetPoint.Body.Position);
            restoreJoint = false;
        }
    }

    private void DragSnot()
    {
        Body.BodyType = (BodyType)1;
        Body.LinearVelocity = Vector2.Zero;
        Vector2 vector = builder.TouchRootVec(movingTouch);
        Vector2 vector2 = targetPoint.Body.Position;
        if (vector2.DistanceTo(vector) > 55f * builder.SizeMult)
        {
            if (Body.Position.FuzzyEquals(targetPoint.Body.Position, 0.1f))
            {
                snot.Physics.EndBody.ApplyForce((Body.Position - vector) * 3f);
            }
            vector2 = vector;
            foreach (SnotPoint snotPoint in Game.SnotPoints)
            {
                if (!snotPoint.Used && vector2.DistanceTo(snotPoint.Body.Position) < 50f * builder.SizeMult)
                {
                    targetPoint.Used = false;
                    vector2 = snotPoint.Body.Position;
                    targetPoint = snotPoint;
                    targetPoint.Used = true;
                }
            }
        }
        targetPosition = VectorExtensions.StepTo(step: Math.Max(targetPosition.DistanceTo(vector2) / 5f, 0.5f), source: targetPosition, target: vector2);
        MoveTo(targetPosition);
    }

    public void MoveTo(Vector2 position)
    {
        Vector2 vector = position - Body.Position;
        Body.Position += vector;
        for (int i = 0; i < snot.Physics.BodiesSize(); i++)
        {
            Body obj = snot.Physics.BodyAt(i);
            obj.Position += vector;
        }
    }

    public override bool TouchBegan(Touch touch)
    {
        _ = base.TouchBegan(touch);
        targetPosition = targetPoint.Body.Position;
        snot.StopParts();
        return true;
    }

    public override void TouchEnd(Touch touch)
    {
        base.TouchEnd(touch);
        EndMove();
    }

    private void EndMove()
    {
        targetSpeed = MathHelper.Clamp(targetPoint.Body.Position.DistanceTo(Body.Position) * 5f, 400f * builder.SizeMult, 1500f * builder.SizeMult);
        moving = false;
    }

    protected override void FreeTouch(Touch touch)
    {
    }

    public void Restart()
    {
        if (moving)
        {
            base.FreeTouch(movingTouch);
        }
        Body.BodyType = (BodyType)1;
        Body.LinearVelocity = Vector2.Zero;
        DestroyEyeJoint();
        snot.Enabled = false;
        targetPoint.Enabled = true;
        targetPoint.Used = false;
        targetPoint = initialPoint;
        targetPoint.Used = true;
        EndMove();
    }
}
