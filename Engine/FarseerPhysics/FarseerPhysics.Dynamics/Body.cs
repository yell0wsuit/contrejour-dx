using System;
using System.Collections.Generic;

using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Common.PhysicsLogic;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics;

public class Body : IDisposable
{
    [ThreadStatic]
    private static int _bodyIdCounter;
    private BodyType _bodyType;

    private float _inertia;
    private float _mass;

    private bool _sleepingAllowed = true;

    private bool _awake = true;

    private bool _fixedRotation;

    internal bool _enabled = true;

    internal float _angularVelocity;

    internal Vector2 _linearVelocity;

    internal Vector2 _force;

    internal float _invI;

    internal float _invMass;

    internal float _sleepTime;

    internal Sweep _sweep;

    internal float _torque;

    internal World _world;

    internal Transform _xf;

    internal bool _island;

    public PhysicsLogicFilter PhysicsLogicFilter { get; set; }

    public ControllerFilter ControllerFilter { get; set; }

    public int BodyId { get; private set; }

    public int IslandIndex { get; set; }

    public float GravityScale { get; set; }

    public object UserData { get; set; }

    public float Revolutions => Rotation / (float)Math.PI;

    public BodyType BodyType
    {
        get => _bodyType;
        set
        {
            if (_bodyType == value)
            {
                return;
            }
            _bodyType = value;
            ResetMassData();
            if (_bodyType == BodyType.Static)
            {
                _linearVelocity = Vector2.Zero;
                _angularVelocity = 0f;
                _sweep.A0 = _sweep.A;
                _sweep.C0 = _sweep.C;
                SynchronizeFixtures();
            }
            Awake = true;
            _force = Vector2.Zero;
            _torque = 0f;
            ContactEdge contactEdge = ContactList;
            while (contactEdge != null)
            {
                ContactEdge contactEdge2 = contactEdge;
                contactEdge = contactEdge.Next;
                _world.ContactManager.Destroy(contactEdge2.Contact);
            }
            ContactList = null;
            IBroadPhase broadPhase = _world.ContactManager.BroadPhase;
            foreach (Fixture fixture in FixtureList)
            {
                int proxyCount = fixture.ProxyCount;
                for (int i = 0; i < proxyCount; i++)
                {
                    broadPhase.TouchProxy(fixture.Proxies[i].ProxyId);
                }
            }
        }
    }

    public Vector2 LinearVelocity
    {
        get => _linearVelocity;
        set
        {
            if (_bodyType != BodyType.Static)
            {
                if (Vector2.Dot(value, value) > 0f)
                {
                    Awake = true;
                }
                _linearVelocity = value;
            }
        }
    }

    public float AngularVelocity
    {
        get => _angularVelocity;
        set
        {
            if (_bodyType != BodyType.Static)
            {
                if (value * value > 0f)
                {
                    Awake = true;
                }
                _angularVelocity = value;
            }
        }
    }

    public float LinearDamping { get; set; }

    public float AngularDamping { get; set; }

    public bool IsBullet { get; set; }

    public bool SleepingAllowed
    {
        get => _sleepingAllowed;
        set
        {
            if (!value)
            {
                Awake = true;
            }
            _sleepingAllowed = value;
        }
    }

    public bool Awake
    {
        get => _awake;
        set
        {
            if (value)
            {
                if (!_awake)
                {
                    _sleepTime = 0f;
                    ContactManager.UpdateContacts();
                }
            }
            else
            {
                ResetDynamics();
                _sleepTime = 0f;
                ContactManager.UpdateContacts();
            }
            _awake = value;
        }
    }

    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (value == _enabled)
            {
                return;
            }
            if (value)
            {
                IBroadPhase broadPhase = _world.ContactManager.BroadPhase;
                for (int i = 0; i < FixtureList.Count; i++)
                {
                    FixtureList[i].CreateProxies(broadPhase, ref _xf);
                }
            }
            else
            {
                IBroadPhase broadPhase2 = _world.ContactManager.BroadPhase;
                for (int j = 0; j < FixtureList.Count; j++)
                {
                    FixtureList[j].DestroyProxies(broadPhase2);
                }
                ContactEdge contactEdge = ContactList;
                while (contactEdge != null)
                {
                    ContactEdge contactEdge2 = contactEdge;
                    contactEdge = contactEdge.Next;
                    _world.ContactManager.Destroy(contactEdge2.Contact);
                }
                ContactList = null;
            }
            _enabled = value;
        }
    }

    public bool FixedRotation
    {
        get => _fixedRotation;
        set
        {
            if (_fixedRotation != value)
            {
                _fixedRotation = value;
                _angularVelocity = 0f;
                ResetMassData();
            }
        }
    }

    public List<Fixture> FixtureList { get; internal set; }

    public JointEdge JointList { get; internal set; }

    public ContactEdge ContactList { get; internal set; }

    public Vector2 Position
    {
        get => _xf.p;
        set => SetTransform(ref value, Rotation);
    }

    public float Rotation
    {
        get => _sweep.A;
        set => SetTransform(ref _xf.p, value);
    }

    public bool IsStatic
    {
        get => _bodyType == BodyType.Static;
        set => BodyType = (!value) ? BodyType.Dynamic : BodyType.Static;
    }

    public bool IsKinematic
    {
        get => _bodyType == BodyType.Kinematic;
        set => BodyType = value ? BodyType.Kinematic : BodyType.Dynamic;
    }

    public bool IgnoreGravity { get; set; }

    public Vector2 WorldCenter => _sweep.C;

    public Vector2 LocalCenter
    {
        get => _sweep.LocalCenter;
        set
        {
            if (_bodyType == BodyType.Dynamic)
            {
                Vector2 c = _sweep.C;
                _sweep.LocalCenter = value;
                _sweep.C0 = _sweep.C = MathUtils.Mul(ref _xf, ref _sweep.LocalCenter);
                Vector2 vector = _sweep.C - c;
                _linearVelocity += new Vector2((0f - _angularVelocity) * vector.Y, _angularVelocity * vector.X);
            }
        }
    }

    public float Mass
    {
        get => _mass;
        set
        {
            if (_bodyType == BodyType.Dynamic)
            {
                _mass = value;
                if (_mass <= 0f)
                {
                    _mass = 1f;
                }
                _invMass = 1f / _mass;
            }
        }
    }

    public float Inertia
    {
        get => _inertia + (Mass * Vector2.Dot(_sweep.LocalCenter, _sweep.LocalCenter));
        set
        {
            if (_bodyType == BodyType.Dynamic && value > 0f && !_fixedRotation)
            {
                _inertia = value - (Mass * Vector2.Dot(LocalCenter, LocalCenter));
                _invI = 1f / _inertia;
            }
        }
    }

    public float Restitution
    {
        get
        {
            float num = 0f;
            for (int i = 0; i < FixtureList.Count; i++)
            {
                Fixture fixture = FixtureList[i];
                num += fixture.Restitution;
            }
            return FixtureList.Count <= 0 ? 0f : num / FixtureList.Count;
        }
        set
        {
            for (int i = 0; i < FixtureList.Count; i++)
            {
                Fixture fixture = FixtureList[i];
                fixture.Restitution = value;
            }
        }
    }

    public float Friction
    {
        get
        {
            float num = 0f;
            for (int i = 0; i < FixtureList.Count; i++)
            {
                Fixture fixture = FixtureList[i];
                num += fixture.Friction;
            }
            return FixtureList.Count <= 0 ? 0f : num / FixtureList.Count;
        }
        set
        {
            for (int i = 0; i < FixtureList.Count; i++)
            {
                Fixture fixture = FixtureList[i];
                fixture.Friction = value;
            }
        }
    }

    public Category CollisionCategories
    {
        set
        {
            for (int i = 0; i < FixtureList.Count; i++)
            {
                Fixture fixture = FixtureList[i];
                fixture.CollisionCategories = value;
            }
        }
    }

    public Category CollidesWith
    {
        set
        {
            for (int i = 0; i < FixtureList.Count; i++)
            {
                Fixture fixture = FixtureList[i];
                fixture.CollidesWith = value;
            }
        }
    }

    public Category IgnoreCCDWith
    {
        set
        {
            for (int i = 0; i < FixtureList.Count; i++)
            {
                Fixture fixture = FixtureList[i];
                fixture.IgnoreCCDWith = value;
            }
        }
    }

    public short CollisionGroup
    {
        set
        {
            for (int i = 0; i < FixtureList.Count; i++)
            {
                Fixture fixture = FixtureList[i];
                fixture.CollisionGroup = value;
            }
        }
    }

    public bool IsSensor
    {
        set
        {
            for (int i = 0; i < FixtureList.Count; i++)
            {
                Fixture fixture = FixtureList[i];
                fixture.IsSensor = value;
            }
        }
    }

    public bool IgnoreCCD { get; set; }

    public bool IsDisposed { get; set; }

    public event OnCollisionHandler OnCollision
    {
        add
        {
            for (int i = 0; i < FixtureList.Count; i++)
            {
                Fixture fixture = FixtureList[i];
                fixture.OnCollision = (OnCollisionHandler)Delegate.Combine(fixture.OnCollision, value);
            }
        }
        remove
        {
            for (int i = 0; i < FixtureList.Count; i++)
            {
                Fixture fixture = FixtureList[i];
                fixture.OnCollision = (OnCollisionHandler)Delegate.Remove(fixture.OnCollision, value);
            }
        }
    }

    public event OnSeparationHandler OnSeparation
    {
        add
        {
            for (int i = 0; i < FixtureList.Count; i++)
            {
                Fixture fixture = FixtureList[i];
                fixture.OnSeparation = (OnSeparationHandler)Delegate.Combine(fixture.OnSeparation, value);
            }
        }
        remove
        {
            for (int i = 0; i < FixtureList.Count; i++)
            {
                Fixture fixture = FixtureList[i];
                fixture.OnSeparation = (OnSeparationHandler)Delegate.Remove(fixture.OnSeparation, value);
            }
        }
    }

    public Body(World world, Vector2? position = null, float rotation = 0f, object userdata = null)
    {
        FixtureList = [];
        BodyId = _bodyIdCounter++;
        _world = world;
        UserData = userdata;
        GravityScale = 1f;
        BodyType = BodyType.Static;
        Enabled = true;
        _xf.q.Set(rotation);
        if (position.HasValue)
        {
            _xf.p = position.Value;
            _sweep.C0 = _xf.p;
            _sweep.C = _xf.p;
            _sweep.A0 = rotation;
            _sweep.A = rotation;
        }
        world.AddBody(this);
    }

    public void ResetDynamics()
    {
        _torque = 0f;
        _angularVelocity = 0f;
        _force = Vector2.Zero;
        _linearVelocity = Vector2.Zero;
    }

    public Fixture CreateFixture(Shape shape, object userData = null)
    {
        return new Fixture(this, shape, userData);
    }

    public void DestroyFixture(Fixture fixture)
    {
        ContactEdge contactEdge = ContactList;
        while (contactEdge != null)
        {
            Contact contact = contactEdge.Contact;
            contactEdge = contactEdge.Next;
            Fixture fixtureA = contact.FixtureA;
            Fixture fixtureB = contact.FixtureB;
            if (fixture == fixtureA || fixture == fixtureB)
            {
                _world.ContactManager.Destroy(contact);
            }
        }
        if (_enabled)
        {
            IBroadPhase broadPhase = _world.ContactManager.BroadPhase;
            fixture.DestroyProxies(broadPhase);
        }
        _ = FixtureList.Remove(fixture);
        fixture.Destroy();
        fixture.Body = null;
        ResetMassData();
    }

    public void SetTransform(ref Vector2 position, float rotation)
    {
        SetTransformIgnoreContacts(ref position, rotation);
        _world.ContactManager.FindNewContacts();
    }

    public void SetTransform(Vector2 position, float rotation)
    {
        SetTransform(ref position, rotation);
    }

    public void SetTransformIgnoreContacts(ref Vector2 position, float angle)
    {
        _xf.q.Set(angle);
        _xf.p = position;
        _sweep.C = MathUtils.Mul(ref _xf, _sweep.LocalCenter);
        _sweep.A = angle;
        _sweep.C0 = _sweep.C;
        _sweep.A0 = angle;
        IBroadPhase broadPhase = _world.ContactManager.BroadPhase;
        for (int i = 0; i < FixtureList.Count; i++)
        {
            FixtureList[i].Synchronize(broadPhase, ref _xf, ref _xf);
        }
    }

    public void GetTransform(out Transform transform)
    {
        transform = _xf;
    }

    public void ApplyForce(Vector2 force, Vector2 point)
    {
        ApplyForce(ref force, ref point);
    }

    public void ApplyForce(ref Vector2 force)
    {
        ApplyForce(ref force, ref _xf.p);
    }

    public void ApplyForce(Vector2 force)
    {
        ApplyForce(ref force, ref _xf.p);
    }

    public void ApplyForce(ref Vector2 force, ref Vector2 point)
    {
        if (_bodyType == BodyType.Dynamic)
        {
            if (!Awake)
            {
                Awake = true;
            }
            _force += force;
            _torque += ((point.X - _sweep.C.X) * force.Y) - ((point.Y - _sweep.C.Y) * force.X);
        }
    }

    public void ApplyTorque(float torque)
    {
        if (_bodyType == BodyType.Dynamic)
        {
            if (!Awake)
            {
                Awake = true;
            }
            _torque += torque;
        }
    }

    public void ApplyLinearImpulse(Vector2 impulse)
    {
        ApplyLinearImpulse(ref impulse);
    }

    public void ApplyLinearImpulse(Vector2 impulse, Vector2 point)
    {
        ApplyLinearImpulse(ref impulse, ref point);
    }

    public void ApplyLinearImpulse(ref Vector2 impulse)
    {
        if (_bodyType == BodyType.Dynamic)
        {
            if (!Awake)
            {
                Awake = true;
            }
            _linearVelocity += _invMass * impulse;
        }
    }

    public void ApplyLinearImpulse(ref Vector2 impulse, ref Vector2 point)
    {
        if (_bodyType == BodyType.Dynamic)
        {
            if (!Awake)
            {
                Awake = true;
            }
            _linearVelocity += _invMass * impulse;
            _angularVelocity += _invI * (((point.X - _sweep.C.X) * impulse.Y) - ((point.Y - _sweep.C.Y) * impulse.X));
        }
    }

    public void ApplyAngularImpulse(float impulse)
    {
        if (_bodyType == BodyType.Dynamic)
        {
            if (!Awake)
            {
                Awake = true;
            }
            _angularVelocity += _invI * impulse;
        }
    }

    public void ResetMassData()
    {
        _mass = 0f;
        _invMass = 0f;
        _inertia = 0f;
        _invI = 0f;
        _sweep.LocalCenter = Vector2.Zero;
        if (BodyType == BodyType.Kinematic)
        {
            _sweep.C0 = _xf.p;
            _sweep.C = _xf.p;
            _sweep.A0 = _sweep.A;
            return;
        }
        Vector2 zero = Vector2.Zero;
        foreach (Fixture fixture in FixtureList)
        {
            if (fixture.Shape._density != 0f)
            {
                MassData massData = fixture.Shape.MassData;
                _mass += massData.Mass;
                zero += massData.Mass * massData.Centroid;
                _inertia += massData.Inertia;
            }
        }
        if (BodyType == BodyType.Static)
        {
            _sweep.C0 = _sweep.C = _xf.p;
            return;
        }
        if (_mass > 0f)
        {
            _invMass = 1f / _mass;
            zero *= _invMass;
        }
        else
        {
            _mass = 1f;
            _invMass = 1f;
        }
        if (_inertia > 0f && !_fixedRotation)
        {
            _inertia -= _mass * Vector2.Dot(zero, zero);
            _invI = 1f / _inertia;
        }
        else
        {
            _inertia = 0f;
            _invI = 0f;
        }
        Vector2 c = _sweep.C;
        _sweep.LocalCenter = zero;
        _sweep.C0 = _sweep.C = MathUtils.Mul(ref _xf, ref _sweep.LocalCenter);
        Vector2 vector = _sweep.C - c;
        _linearVelocity += new Vector2((0f - _angularVelocity) * vector.Y, _angularVelocity * vector.X);
    }

    public Vector2 GetWorldPoint(ref Vector2 localPoint)
    {
        return MathUtils.Mul(ref _xf, ref localPoint);
    }

    public Vector2 GetWorldPoint(Vector2 localPoint)
    {
        return GetWorldPoint(ref localPoint);
    }

    public Vector2 GetWorldVector(ref Vector2 localVector)
    {
        return MathUtils.Mul(_xf.q, localVector);
    }

    public Vector2 GetWorldVector(Vector2 localVector)
    {
        return GetWorldVector(ref localVector);
    }

    public Vector2 GetLocalPoint(ref Vector2 worldPoint)
    {
        return MathUtils.MulT(ref _xf, worldPoint);
    }

    public Vector2 GetLocalPoint(Vector2 worldPoint)
    {
        return GetLocalPoint(ref worldPoint);
    }

    public Vector2 GetLocalVector(ref Vector2 worldVector)
    {
        return MathUtils.MulT(_xf.q, worldVector);
    }

    public Vector2 GetLocalVector(Vector2 worldVector)
    {
        return GetLocalVector(ref worldVector);
    }

    public Vector2 GetLinearVelocityFromWorldPoint(Vector2 worldPoint)
    {
        return GetLinearVelocityFromWorldPoint(ref worldPoint);
    }

    public Vector2 GetLinearVelocityFromWorldPoint(ref Vector2 worldPoint)
    {
        return _linearVelocity + new Vector2((0f - _angularVelocity) * (worldPoint.Y - _sweep.C.Y), _angularVelocity * (worldPoint.X - _sweep.C.X));
    }

    public Vector2 GetLinearVelocityFromLocalPoint(Vector2 localPoint)
    {
        return GetLinearVelocityFromLocalPoint(ref localPoint);
    }

    public Vector2 GetLinearVelocityFromLocalPoint(ref Vector2 localPoint)
    {
        return GetLinearVelocityFromWorldPoint(GetWorldPoint(ref localPoint));
    }

    internal void SynchronizeFixtures()
    {
        Transform transform = default;
        transform.q.Set(_sweep.A0);
        transform.p = _sweep.C0 - MathUtils.Mul(transform.q, _sweep.LocalCenter);
        IBroadPhase broadPhase = _world.ContactManager.BroadPhase;
        for (int i = 0; i < FixtureList.Count; i++)
        {
            FixtureList[i].Synchronize(broadPhase, ref transform, ref _xf);
        }
    }

    internal void SynchronizeTransform()
    {
        _xf.q.Set(_sweep.A);
        _xf.p = _sweep.C - MathUtils.Mul(_xf.q, _sweep.LocalCenter);
    }

    internal bool ShouldCollide(Body other)
    {
        if (_bodyType != BodyType.Dynamic && other._bodyType != BodyType.Dynamic)
        {
            return false;
        }
        for (JointEdge jointEdge = JointList; jointEdge != null; jointEdge = jointEdge.Next)
        {
            if (jointEdge.Other == other && !jointEdge.Joint.CollideConnected)
            {
                return false;
            }
        }
        return true;
    }

    internal void Advance(float alpha)
    {
        _sweep.Advance(alpha);
        _sweep.C = _sweep.C0;
        _sweep.A = _sweep.A0;
        _xf.q.Set(_sweep.A);
        _xf.p = _sweep.C - MathUtils.Mul(_xf.q, _sweep.LocalCenter);
    }

    public void IgnoreCollisionWith(Body other)
    {
        for (int i = 0; i < FixtureList.Count; i++)
        {
            for (int j = 0; j < other.FixtureList.Count; j++)
            {
                FixtureList[i].IgnoreCollisionWith(other.FixtureList[j]);
            }
        }
    }

    public void RestoreCollisionWith(Body other)
    {
        for (int i = 0; i < FixtureList.Count; i++)
        {
            for (int j = 0; j < other.FixtureList.Count; j++)
            {
                FixtureList[i].RestoreCollisionWith(other.FixtureList[j]);
            }
        }
    }

    public void Dispose()
    {
        if (!IsDisposed)
        {
            _world.RemoveBody(this);
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }
    }

    public Body Clone(World world = null)
    {
        Body body = new(world ?? _world, Position, Rotation, UserData)
        {
            _bodyType = _bodyType,
            _linearVelocity = _linearVelocity,
            _angularVelocity = _angularVelocity,
            GravityScale = GravityScale,
            UserData = UserData,
            _enabled = _enabled,
            _fixedRotation = _fixedRotation,
            _sleepingAllowed = _sleepingAllowed,
            LinearDamping = LinearDamping,
            AngularDamping = AngularDamping,
            _awake = _awake,
            IsBullet = IsBullet,
            IgnoreCCD = IgnoreCCD,
            IgnoreGravity = IgnoreGravity,
            _torque = _torque
        };
        return body;
    }

    public Body DeepClone(World world = null)
    {
        Body body = Clone(world ?? _world);
        int count = FixtureList.Count;
        for (int i = 0; i < count; i++)
        {
            _ = FixtureList[i].CloneOnto(body);
        }
        return body;
    }
}
