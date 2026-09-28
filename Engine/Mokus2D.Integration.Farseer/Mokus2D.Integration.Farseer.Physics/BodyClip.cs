using System.Collections.Generic;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Util.Resources;
using Mokus2D.Visual;

namespace Mokus2D.Integration.Farseer.Physics;

public class BodyClip : DisposableBase, IUpdatable
{
    private Body _body;

    public readonly PhysicsUpdater Updater;

    public readonly IDictionary<string, string> Config;

    public Vector2 ClipPosition => Clip.Position;

    public Node Clip { get; protected set; }

    public World World => Updater.World;

    public virtual Body Body
    {
        get
        {
            return _body;
        }
        protected set
        {
            SetBody(value);
        }
    }

    public float BodyAngle => _body.Rotation;

    public BodyClip(PhysicsUpdater updater, Body body, Node clip, IDictionary<string, string> config = null)
    {
        Clip = clip;
        Config = config;
        SetBody(body);
        Updater = updater;
        if (clip != null)
        {
            UpdatePosition(0f);
            UpdateRotation(0f);
        }
    }

    private void SetBody(Body value)
    {
        if (_body != value)
        {
            if (_body != null)
            {
                _body.UserData = null;
            }
            _body = value;
            if (_body != null)
            {
                _ = _body.UserData;
                _body.UserData = this;
            }
        }
    }

    public virtual void Update(float time)
    {
        if (_body != null && Clip != null)
        {
            UpdatePosition(time);
            UpdateRotation(time);
        }
    }

    public virtual void UpdatePosition(float time)
    {
        Clip.Position = Updater.ToPixels(_body.Position);
    }

    public virtual void UpdateRotation(float time)
    {
        Clip.RotationRadians = _body.Rotation;
    }

    public virtual void Clear()
    {
    }

    public virtual void Destroy()
    {
        if (Clip != null && Clip.Parent != null)
        {
            Clip.RemoveFromParent();
        }
        RemoveBody();
    }

    public void RemoveBody()
    {
        _body.UserData = null;
        Updater.World.RemoveBody(_body);
    }

    public virtual void OnCollision(Fixture otherFixture, Contact point)
    {
    }

    public virtual void OnCollisionStart(Fixture otherFixture, Contact point)
    {
    }

    public virtual void OnCollisionEnd(Fixture otherFixture, Contact point)
    {
    }

    public virtual void PostSolvePointImpulse(Fixture otherFixture, Contact point, ContactVelocityConstraint impulse)
    {
    }
}
