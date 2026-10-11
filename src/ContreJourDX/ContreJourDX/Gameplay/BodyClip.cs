using System;
using System.Collections.Generic;
using System.Numerics;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Mokus2D.Util;
using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class BodyClip : Updatable
    {
        public float RotationOffset { get; set; }

        protected float RotationOffsetRadians { get; set; }

        private Body body;

        public Node Clip { get; protected set; }

        public Hashtable Config { get; set; }

        public LevelBuilderBase Builder { get; protected set; }

        private bool destroyed;

        private bool destroyLaterCalled;

        private readonly Flag _firstUpdate = new();

        public World World => Builder.World;

        protected float InitialBodyAngle => float.DegreesToRadians(0f - RotationOffset);

        public float BodyAngle => InitialBodyAngle + body.Rotation;

        public virtual Vector2 PositionVec => body.Position;

        public Vector2 Position => Builder.ToPoint(body.Position);

        public virtual Body Body
        {
            get => body;
            protected set
            {
                body?.UserData = null;
                body = value;
                if (body != null)
                {
                    _ = body.UserData;
                    body.UserData = this;
                }
            }
        }

        public BodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
        {
            //IL_0056: Unknown result type (might be due to invalid IL or missing references)
            //IL_0060: Expected O, but got Unknown
            config ??= [];
            RotationOffset = config.GetFloat("rotationOffset", 0f);
            RotationOffsetRadians = float.DegreesToRadians(RotationOffset);
            Config = config;
            Clip = clip;
            Body = (Body)body;
            Builder = builder;
        }

        public void Schedule(Action action, float delay)
        {
            ((ContreJourDXGame)Builder.Game).Schedule(action, delay);
        }

        public void UnSchedule(Action action)
        {
            ((ContreJourDXGame)Builder.Game).UnSchedule(action);
        }

        public static List<string> TexturesToUnload()
        {
            return [];
        }

        public override void Update(float time)
        {
            if (_firstUpdate.Use())
            {
                FirstUpdate();
            }
            if (body != null && Clip != null)
            {
                UpdatePosition();
                UpdateRotation();
            }
        }

        protected virtual void FirstUpdate()
        {
        }

        public virtual void UpdatePosition()
        {
            Clip.Position = Builder.ToPoint(body.Position);
        }

        public virtual void UpdateRotation()
        {
            Clip.RotationRadians = body.Rotation - RotationOffsetRadians;
        }

        public void DestroyLater()
        {
            if (!destroyLaterCalled)
            {
                destroyLaterCalled = true;
                Schedule(Destroy, 0.01f);
            }
        }

        public virtual void Clear()
        {
        }

        public void Destroy()
        {
            if (Clip != null && Clip.Parent != null)
            {
                Clip.Parent.RemoveChild(Clip);
            }
            RemoveBody();
        }

        public void RemoveBody()
        {
            if (!destroyed)
            {
                destroyed = true;
                body.UserData = null;
                World.RemoveBody(body);
            }
        }

        public virtual void OnCollisionPoint(Body body2, Contact point)
        {
        }

        public virtual void OnCollisionStartPoint(Body body2, Contact point)
        {
        }

        public virtual void OnCollisionEndPoint(Body body2, Contact point)
        {
        }

        public virtual void PostSolvePointImpulse(Body body2, Contact point, ContactVelocityConstraint impulse)
        {
        }
    }
}
