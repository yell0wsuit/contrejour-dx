using System;
using System.Collections.Generic;

using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Contacts;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics;

public class Fixture : IDisposable
{
    [ThreadStatic]
    private static int _fixtureIdCounter;

    private bool _isSensor;

    private float _friction;

    private float _restitution;

    internal Category _collidesWith;

    internal Category _collisionCategories;

    internal short _collisionGroup;

    internal HashSet<int> _collisionIgnores;

    public FixtureProxy[] Proxies;

    public int ProxyCount;

    public Category IgnoreCCDWith;

    public AfterCollisionEventHandler AfterCollision;

    public BeforeCollisionEventHandler BeforeCollision;

    public OnCollisionEventHandler OnCollision;

    public OnSeparationEventHandler OnSeparation;

    public short CollisionGroup
    {
        get => _collisionGroup;
        set
        {
            if (_collisionGroup != value)
            {
                _collisionGroup = value;
                Refilter();
            }
        }
    }

    public Category CollidesWith
    {
        get => _collidesWith;
        set
        {
            if (_collidesWith != value)
            {
                _collidesWith = value;
                Refilter();
            }
        }
    }

    public Category CollisionCategories
    {
        get => _collisionCategories;
        set
        {
            if (_collisionCategories != value)
            {
                _collisionCategories = value;
                Refilter();
            }
        }
    }

    public Shape Shape { get; internal set; }

    public bool IsSensor
    {
        get => _isSensor;
        set
        {
            Body?.Awake = true;
            _isSensor = value;
        }
    }

    public Body Body { get; internal set; }

    public object UserData { get; set; }

    public float Friction
    {
        get => _friction;
        set => _friction = value;
    }

    public float Restitution
    {
        get => _restitution;
        set => _restitution = value;
    }

    public int FixtureId { get; internal set; }

    public bool IsDisposed { get; set; }

    internal Fixture()
    {
        FixtureId = _fixtureIdCounter++;
        _collisionCategories = Settings.DefaultFixtureCollisionCategories;
        _collidesWith = Settings.DefaultFixtureCollidesWith;
        _collisionGroup = 0;
        _collisionIgnores = [];
        IgnoreCCDWith = Settings.DefaultFixtureIgnoreCCDWith;
        Friction = 0.2f;
        Restitution = 0f;
    }

    internal Fixture(Body body, Shape shape, object userData = null)
        : this()
    {
        Body = body;
        UserData = userData;
        Shape = shape.Clone();
        RegisterFixture();
    }

    public void Dispose()
    {
        if (!IsDisposed)
        {
            Body.DestroyFixture(this);
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }
    }

    public void RestoreCollisionWith(Fixture fixture)
    {
        if (_collisionIgnores.Contains(fixture.FixtureId))
        {
            _ = _collisionIgnores.Remove(fixture.FixtureId);
            Refilter();
        }
    }

    public void IgnoreCollisionWith(Fixture fixture)
    {
        if (!_collisionIgnores.Contains(fixture.FixtureId))
        {
            _ = _collisionIgnores.Add(fixture.FixtureId);
            Refilter();
        }
    }

    public bool IsFixtureIgnored(Fixture fixture)
    {
        return _collisionIgnores.Contains(fixture.FixtureId);
    }

    private void Refilter()
    {
        for (ContactEdge contactEdge = Body.ContactList; contactEdge != null; contactEdge = contactEdge.Next)
        {
            Contact contact = contactEdge.Contact;
            Fixture fixtureA = contact.FixtureA;
            Fixture fixtureB = contact.FixtureB;
            if (fixtureA == this || fixtureB == this)
            {
                contact.FilterFlag = true;
            }
        }
        World world = Body._world;
        if (world != null)
        {
            IBroadPhase broadPhase = world.ContactManager.BroadPhase;
            for (int i = 0; i < ProxyCount; i++)
            {
                broadPhase.TouchProxy(Proxies[i].ProxyId);
            }
        }
    }

    private void RegisterFixture()
    {
        Proxies = new FixtureProxy[Shape.ChildCount];
        ProxyCount = 0;
        if (Body.Enabled)
        {
            IBroadPhase broadPhase = Body._world.ContactManager.BroadPhase;
            CreateProxies(broadPhase, ref Body._xf);
        }
        Body.FixtureList.Add(this);
        if (Shape._density > 0f)
        {
            Body.ResetMassData();
        }
        Body._world._worldHasNewFixture = true;
        Body._world.FixtureAdded?.Invoke(this);
    }

    public bool TestPoint(ref Vector2 point)
    {
        return Shape.TestPoint(ref Body._xf, ref point);
    }

    public bool RayCast(out RayCastOutput output, ref RayCastInput input, int childIndex)
    {
        return Shape.RayCast(out output, ref input, ref Body._xf, childIndex);
    }

    public void GetAABB(out AABB aabb, int childIndex)
    {
        aabb = Proxies[childIndex].AABB;
    }

    internal void Destroy()
    {
        Proxies = null;
        Shape = null;
        UserData = null;
        BeforeCollision = null;
        OnCollision = null;
        OnSeparation = null;
        AfterCollision = null;
        Body._world.FixtureRemoved?.Invoke(this);
        Body._world.FixtureAdded = null;
        Body._world.FixtureRemoved = null;
        OnSeparation = null;
        OnCollision = null;
    }

    internal void CreateProxies(IBroadPhase broadPhase, ref Transform xf)
    {
        ProxyCount = Shape.ChildCount;
        for (int i = 0; i < ProxyCount; i++)
        {
            FixtureProxy proxy = default;
            Shape.ComputeAABB(out proxy.AABB, ref xf, i);
            proxy.Fixture = this;
            proxy.ChildIndex = i;
            proxy.ProxyId = broadPhase.AddProxy(ref proxy);
            Proxies[i] = proxy;
        }
    }

    internal void DestroyProxies(IBroadPhase broadPhase)
    {
        for (int i = 0; i < ProxyCount; i++)
        {
            broadPhase.RemoveProxy(Proxies[i].ProxyId);
            Proxies[i].ProxyId = -1;
        }
        ProxyCount = 0;
    }

    internal void Synchronize(IBroadPhase broadPhase, ref Transform transform1, ref Transform transform2)
    {
        if (ProxyCount != 0)
        {
            for (int i = 0; i < ProxyCount; i++)
            {
                FixtureProxy fixtureProxy = Proxies[i];
                Shape.ComputeAABB(out AABB aabb, ref transform1, fixtureProxy.ChildIndex);
                Shape.ComputeAABB(out AABB aabb2, ref transform2, fixtureProxy.ChildIndex);
                fixtureProxy.AABB.Combine(ref aabb, ref aabb2);
                Vector2 displacement = transform2.p - transform1.p;
                broadPhase.MoveProxy(fixtureProxy.ProxyId, ref fixtureProxy.AABB, displacement);
            }
        }
    }

    internal bool CompareTo(Fixture fixture)
    {
        return _collidesWith == fixture._collidesWith && _collisionCategories == fixture._collisionCategories && _collisionGroup == fixture._collisionGroup && Friction == fixture.Friction && IsSensor == fixture.IsSensor && Restitution == fixture.Restitution && UserData == fixture.UserData && IgnoreCCDWith == fixture.IgnoreCCDWith && SequenceEqual(_collisionIgnores, fixture._collisionIgnores);
    }

    private bool SequenceEqual<T>(HashSet<T> first, HashSet<T> second)
    {
        if (first.Count != second.Count)
        {
            return false;
        }
        using HashSet<T>.Enumerator enumerator = first.GetEnumerator();
        using HashSet<T>.Enumerator enumerator2 = second.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (!enumerator2.MoveNext() || !Equals(enumerator.Current, enumerator2.Current))
            {
                return false;
            }
        }
        return !enumerator2.MoveNext();
    }

    public Fixture CloneOnto(Body body)
    {
        Fixture fixture = new()
        {
            Body = body,
            Shape = Shape.Clone(),
            UserData = UserData,
            Restitution = Restitution,
            Friction = Friction,
            IsSensor = IsSensor,
            _collisionGroup = _collisionGroup,
            _collisionCategories = _collisionCategories,
            _collidesWith = _collidesWith,
            IgnoreCCDWith = IgnoreCCDWith
        };
        foreach (int collisionIgnore in _collisionIgnores)
        {
            _ = fixture._collisionIgnores.Add(collisionIgnore);
        }
        fixture.RegisterFixture();
        return fixture;
    }
}
