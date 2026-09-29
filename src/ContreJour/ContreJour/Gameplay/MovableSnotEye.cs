using System;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

using Microsoft.Xna.Framework;

using Mokus2D.Input;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace ContreJour.Gameplay
{
    public class MovableSnotEye(SnotBodyClip snot, Body body, SnotPoint targetPoint) : SnotEye(snot, body), IRestartable
    {
        private bool moving;

        private Touch movingTouch;

        private SnotPoint targetPoint = targetPoint;

        private bool restoreJoint;

        private float targetSpeed;

        private readonly SnotPoint initialPoint = targetPoint;

        private Vector2 targetPosition;

        public override int Priority(Vector2 touchPosition)
        {
            return !Snot.Joined ? 0 : 1;
        }

        protected override void CheckTouchDistance(Touch touch, float distance)
        {
            if (distance > 55f * Builder.SizeMult && Snot.Linked == null)
            {
                HasRelease = false;
                moving = true;
                movingTouch = touch;
                Snot.Enabled = false;
                DestroyEyeJoint();
                Snot.Physics.EyeJoint = null;
            }
            else
            {
                base.CheckTouchDistance(touch, distance);
            }
        }

        private void DestroyEyeJoint()
        {
            if (Snot.Physics.EyeJoint != null)
            {
                restoreJoint = true;
                Builder.World.RemoveJoint((Joint)(object)Snot.Physics.EyeJoint);
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
            else if (!Snot.Enabled)
            {
                EndSnotDrag();
            }
            targetPoint?.Enabled = Snot.Linked == null;
        }

        private void EndSnotDrag()
        {
            Body.BodyType = (BodyType)2;
            Snot.Enabled = true;
            if (restoreJoint)
            {
                Snot.Physics.EyeJoint = FarseerUtil.CreateRevoluteJoint(Builder.World, Snot.Physics.JoinedBody, Snot.Physics.EyeBody, targetPoint.Body.Position);
                restoreJoint = false;
            }
        }

        private void DragSnot()
        {
            Body.BodyType = (BodyType)1;
            Body.LinearVelocity = Vector2.Zero;
            Vector2 vector = Builder.TouchRootVec(movingTouch);
            Vector2 vector2 = targetPoint.Body.Position;
            if (Vector2.Distance(vector2, vector) > 55f * Builder.SizeMult)
            {
                if (Body.Position.FuzzyEquals(targetPoint.Body.Position, 0.1f))
                {
                    Snot.Physics.EndBody.ApplyForce((Body.Position - vector) * 3f);
                }
                vector2 = vector;
                foreach (SnotPoint snotPoint in Game.SnotPoints)
                {
                    if (!snotPoint.Used && Vector2.Distance(vector2, snotPoint.Body.Position) < 50f * Builder.SizeMult)
                    {
                        targetPoint.Used = false;
                        vector2 = snotPoint.Body.Position;
                        targetPoint = snotPoint;
                        targetPoint.Used = true;
                    }
                }
            }
            targetPosition = VectorExtensions.StepTo(step: Math.Max(Vector2.Distance(targetPosition, vector2) / 5f, 0.5f), source: targetPosition, target: vector2);
            MoveTo(targetPosition);
        }

        public void MoveTo(Vector2 position)
        {
            Vector2 vector = position - Body.Position;
            Body.Position += vector;
            for (int i = 0; i < Snot.Physics.BodiesSize(); i++)
            {
                Body obj = Snot.Physics.BodyAt(i);
                obj.Position += vector;
            }
        }

        public override bool TouchBegan(Touch touch)
        {
            _ = base.TouchBegan(touch);
            targetPosition = targetPoint.Body.Position;
            Snot.StopParts();
            return true;
        }

        public override void TouchEnd(Touch touch)
        {
            base.TouchEnd(touch);
            EndMove();
        }

        private void EndMove()
        {
            targetSpeed = Math.Clamp(Vector2.Distance(targetPoint.Body.Position, Body.Position) * 5f, 400f * Builder.SizeMult, 1500f * Builder.SizeMult);
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
            Snot.Enabled = false;
            targetPoint.Enabled = true;
            targetPoint.Used = false;
            targetPoint = initialPoint;
            targetPoint.Used = true;
            EndMove();
        }
    }
}
