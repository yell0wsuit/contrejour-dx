using System;
using System.Collections.Generic;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Microsoft.Xna.Framework;

using Mokus2D.Util;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Default.Namespace;

public class BodyClip : Updatable
{
    protected float rotationOffset;

    protected float rotationOffsetRadians;

    private Body body;

    protected Node clip;

    protected Hashtable config;

    protected LevelBuilderBase builder;

    protected bool destroyed;

    protected bool destroyLaterCalled;

    private readonly Flag _firstUpdate = new();

    public Hashtable Config
    {
        get => config;
        set => config = value;
    }

    public Node Clip => clip;

    public float RotationOffset
    {
        get => rotationOffset;
        set => rotationOffset = value;
    }

    public LevelBuilderBase Builder => builder;

    public World World => builder.World;

    protected float InitialBodyAngle => MathHelper.ToRadians(0f - rotationOffset);

    public float BodyAngle => InitialBodyAngle + body.Rotation;

    public virtual Vector2 PositionVec => body.Position;

    public Vector2 Position => builder.ToPoint(body.Position);

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
        rotationOffset = config.GetFloat("rotationOffset", 0f);
        rotationOffsetRadians = rotationOffset.ToRadians();
        this.config = config;
        this.clip = clip;
        Body = (Body)body;
        this.builder = builder;
    }

    public void Schedule(Action action, float delay)
    {
        ((ContreJourGame)builder.Game).Schedule(action, delay);
    }

    public void UnSchedule(Action action)
    {
        ((ContreJourGame)builder.Game).UnSchedule(action);
    }

    public List<string> TexturesToUnload()
    {
        return [];
    }

    private Vector2 Scale()
    {
        return config.GetVector("scale", Vector2.One);
    }

    public override void Update(float time)
    {
        if (_firstUpdate.Use())
        {
            FirstUpdate();
        }
        if (body != null && clip != null)
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
        clip.Position = builder.ToPoint(body.Position);
    }

    public virtual void UpdateRotation()
    {
        clip.RotationRadians = body.Rotation - rotationOffsetRadians;
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
        if (clip != null && clip.Parent != null)
        {
            clip.Parent.RemoveChild(clip);
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
