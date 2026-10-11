using System;
using System.Numerics;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;

using Mokus2D.Util.MathUtils;

namespace ContreJourDX.Gameplay
{
    // Web physics uses downward-positive Y; all tail vectors here use DX's upward-positive Y.
    public sealed class BaloonTail
    {
        private const float FirstLength = 1f;
        private const float SecondLength = 0.6f;
        private const float FullLength = FirstLength + SecondLength;
        private readonly World world;
        private readonly Body baloon;
        private readonly DistanceJoint middleJoint;
        private readonly DistanceJoint endJoint;
        private RevoluteJoint linkedJoint;
        private Vector2 targetEnd = new(0f, -FullLength);
        private Vector2 targetMiddle;
        private Vector2 middleFawn;
        private Vector2 endFawn;
        private float targetLength = FullLength;
        private float touchTime;
        private float lastOnGroundTime;
        private float phase = Maths.Random(0f, MathF.PI);
        private float amplitude = 1f;
        private float oscillatorAmplitude = 1f;
        private bool limitAngles;
        private float timeCoeff;

        public Body Start { get; }
        public Body Middle { get; }
        public Body End { get; }
        public bool Linked { get; private set; }
        public bool Dragging { get; private set; }
        public bool Springing { get; private set; }
        public bool Visible { get; set; } = true;
        public Vector2 TargetPosition { get; set; }

        public BaloonTail(World world, Body baloon)
        {
            this.world = world;
            this.baloon = baloon;
            Start = CreatePart(baloon.Position);
            Middle = CreatePart(baloon.Position - new Vector2(0f, FirstLength / 3f));
            End = CreatePart(Middle.Position - new Vector2(0f, SecondLength / 3f));
            middleJoint = FarseerUtil.CreateDistanceJoint(world, Start, Middle, 4f, 1f);
            endJoint = FarseerUtil.CreateDistanceJoint(world, Middle, End, 4f, 1f);
        }

        private Body CreatePart(Vector2 position)
        {
            Body part = world.CreateCircle(4f / 30f, position, density: 0.3f);
            part.BodyType = BodyType.Kinematic;
            part.SetSensor(true);
            return part;
        }

        public void SetLinked(bool value)
        {
            if (Linked == value)
            {
                return;
            }
            Linked = value;
            if (value)
            {
                Start.Position = baloon.Position;
                linkedJoint = FarseerUtil.CreateRevoluteJoint(world, baloon, Start, Start.Position);
                Start.BodyType = BodyType.Dynamic;
                End.BodyType = BodyType.Dynamic;
                targetLength = FullLength * 2f / 3f;
            }
            else
            {
                if (linkedJoint != null)
                {
                    world.RemoveJoint(linkedJoint);
                    linkedJoint = null;
                }
                Start.BodyType = BodyType.Kinematic;
                End.BodyType = BodyType.Kinematic;
                targetLength = FullLength / 3f;
            }
        }

        public void SetDragging(bool value, float totalTime)
        {
            if (Dragging != value)
            {
                Dragging = value;
                SpringTail(totalTime);
            }
        }

        public void SetPositions(Vector2 position)
        {
            Start.Position = Middle.Position = End.Position = position;
        }

        public void SpringTail(float totalTime)
        {
            touchTime = totalTime;
            if (!Dragging)
            {
                Springing = true;
                amplitude = 0f;
                End.BodyType = BodyType.Dynamic;
            }
            Middle.BodyType = BodyType.Dynamic;
            Middle.LinearVelocity = End.LinearVelocity = Vector2.Zero;
        }

        public void Update(float delta, float totalTime)
        {
            if (delta <= 0f)
            {
                return;
            }
            for (ContactEdge contact = baloon.ContactList; contact != null; contact = contact.Next)
            {
                if (contact.Contact.IsTouching && !contact.Contact.IsSensor() && contact.Other.BodyType != BodyType.Dynamic && contact.Other.Position.Y > baloon.Position.Y)
                {
                    lastOnGroundTime = totalTime;
                }
            }
            float speed = Start.LinearVelocity.Length();
            timeCoeff = Maths.StepTo(timeCoeff, Math.Min(2.5f * speed, 15f), 0.1f);
            phase += delta * (1f + timeCoeff) * 1.4f;
            amplitude = Maths.StepTo(amplitude, 1f - (timeCoeff / 20f), 0.01f);
            float angle = speed > 0.2f ? MathF.Atan2(Start.LinearVelocity.Y, Start.LinearVelocity.X) + MathF.PI : MathF.PI / 2f;
            angle = angle.SimplifyAngle(-MathF.PI / 2f);
            if (limitAngles)
            {
                angle = Maths.Clamp(angle, MathF.PI / 6f, MathF.PI * 5f / 6f);
            }
            Vector2 neededEnd = VectorUtil.ToVector((34f + (14f * Math.Min(speed / 3f, 1f))) / 30f, angle);
            float step = Math.Max(speed * delta * 1.25f, 0.01f);
            targetMiddle = StepVector(targetMiddle, targetEnd * FirstLength / FullLength, step);
            targetEnd = StepVector(targetEnd, neededEnd, step);
            Vector2 perpendicular = VectorUtil.ToVector(7f / 30f, MathF.Atan2(targetEnd.Y, targetEnd.X) - (MathF.PI / 2f));
            // Preserve CosChanger's biased amplitude interpolation, reflected into DX's Y axis.
            middleFawn = perpendicular * ((oscillatorAmplitude * (1f + MathF.Cos(phase + (MathF.PI / 2f)))) - 1f);
            endFawn = perpendicular * ((oscillatorAmplitude * (1f + MathF.Cos(phase))) - 1f);
            oscillatorAmplitude = amplitude;
            middleJoint.Length = Maths.StepTo(middleJoint.Length, targetLength * FirstLength / FullLength, 0.05f);
            endJoint.Length = Maths.StepTo(endJoint.Length, targetLength * SecondLength / FullLength, 0.05f);
            if (Linked)
            {
                return;
            }
            limitAngles = totalTime - lastOnGroundTime < 0.3f;
            Start.SetTransform(baloon.Position, baloon.Rotation);
            Start.LinearVelocity = baloon.LinearVelocity;
            if (Springing)
            {
                Vector2 difference = Start.Position - End.Position;
                if (difference.Length() > 0.2f)
                {
                    End.LinearVelocity = StepVector(End.LinearVelocity, difference / delta, 1.5f);
                }
                else
                {
                    Springing = false;
                    End.BodyType = BodyType.Kinematic;
                    targetMiddle = Middle.Position - Start.Position;
                    targetEnd = End.Position - Start.Position;
                    amplitude = 0f;
                }
            }
            else if (Dragging)
            {
                End.LinearVelocity = (TargetPosition - End.Position) * (2f * Math.Min(totalTime - touchTime, 0.25f) / delta);
            }
            else
            {
                End.SetTransform(Start.Position + targetEnd + endFawn, 0f);
                Middle.SetTransform(Start.Position + targetMiddle + middleFawn, 0f);
            }
        }

        private static Vector2 StepVector(Vector2 current, Vector2 target, float step)
        {
            Vector2 difference = target - current;
            return difference.Length() <= step ? target : current + (Vector2.Normalize(difference) * step);
        }

        public void Clear()
        {
            SetLinked(false);
            world.RemoveJoint(middleJoint);
            world.RemoveJoint(endJoint);
            world.RemoveBody(Start);
            world.RemoveBody(Middle);
            world.RemoveBody(End);
        }
    }
}
