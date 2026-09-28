using System;
using System.Collections.Generic;

using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Contacts;

public class Contact
{
    private enum ContactType
    {
        NotSupported,
        Polygon,
        PolygonAndCircle,
        Circle,
        EdgeAndPolygon,
        EdgeAndCircle,
        ChainAndPolygon,
        ChainAndCircle
    }

    private ContactType _type;

    private static EdgeShape _edge = new();

    private static ContactType[,] _registers = new ContactType[4, 4]
    {
        {
            ContactType.Circle,
            ContactType.EdgeAndCircle,
            ContactType.PolygonAndCircle,
            ContactType.ChainAndCircle
        },
        {
            ContactType.EdgeAndCircle,
            ContactType.NotSupported,
            ContactType.EdgeAndPolygon,
            ContactType.NotSupported
        },
        {
            ContactType.PolygonAndCircle,
            ContactType.EdgeAndPolygon,
            ContactType.Polygon,
            ContactType.ChainAndPolygon
        },
        {
            ContactType.ChainAndCircle,
            ContactType.NotSupported,
            ContactType.ChainAndPolygon,
            ContactType.NotSupported
        }
    };

    internal ContactEdge _nodeA = new();

    internal ContactEdge _nodeB = new();

    internal int _toiCount;

    internal float _toi;

    public Fixture FixtureA;

    public Fixture FixtureB;

    public Manifold Manifold;

    public float Friction { get; set; }

    public float Restitution { get; set; }

    public float TangentSpeed { get; set; }

    public bool Enabled { get; set; }

    public int ChildIndexA { get; internal set; }

    public int ChildIndexB { get; internal set; }

    public bool IsTouching { get; set; }

    internal bool IslandFlag { get; set; }

    internal bool TOIFlag { get; set; }

    internal bool FilterFlag { get; set; }

    public void ResetRestitution()
    {
        Restitution = Settings.MixRestitution(FixtureA.Restitution, FixtureB.Restitution);
    }

    public void ResetFriction()
    {
        Friction = Settings.MixFriction(FixtureA.Friction, FixtureB.Friction);
    }

    private Contact(Fixture fA, int indexA, Fixture fB, int indexB)
    {
        Reset(fA, indexA, fB, indexB);
    }

    public void GetWorldManifold(out Vector2 normal, out FixedArray2<Vector2> points)
    {
        Body body = FixtureA.Body;
        Body body2 = FixtureB.Body;
        Shape shape = FixtureA.Shape;
        Shape shape2 = FixtureB.Shape;
        ContactSolver.WorldManifold.Initialize(ref Manifold, ref body._xf, shape.Radius, ref body2._xf, shape2.Radius, out normal, out points);
    }

    private void Reset(Fixture fA, int indexA, Fixture fB, int indexB)
    {
        Enabled = true;
        IsTouching = false;
        IslandFlag = false;
        FilterFlag = false;
        TOIFlag = false;
        FixtureA = fA;
        FixtureB = fB;
        ChildIndexA = indexA;
        ChildIndexB = indexB;
        Manifold.PointCount = 0;
        _nodeA.Contact = null;
        _nodeA.Prev = null;
        _nodeA.Next = null;
        _nodeA.Other = null;
        _nodeB.Contact = null;
        _nodeB.Prev = null;
        _nodeB.Next = null;
        _nodeB.Other = null;
        _toiCount = 0;
        if (FixtureA != null && FixtureB != null)
        {
            Friction = Settings.MixFriction(FixtureA.Friction, FixtureB.Friction);
            Restitution = Settings.MixRestitution(FixtureA.Restitution, FixtureB.Restitution);
        }
        TangentSpeed = 0f;
    }

    internal void Update(ContactManager contactManager)
    {
        Body body = FixtureA.Body;
        Body body2 = FixtureB.Body;
        if (FixtureA == null || FixtureB == null)
        {
            return;
        }
        Manifold oldManifold = Manifold;
        Enabled = true;
        bool isTouching = IsTouching;
        bool flag = FixtureA.IsSensor || FixtureB.IsSensor;
        bool flag2;
        if (flag)
        {
            Shape shape = FixtureA.Shape;
            Shape shape2 = FixtureB.Shape;
            flag2 = Collision.Collision.TestOverlap(shape, ChildIndexA, shape2, ChildIndexB, ref body._xf, ref body2._xf);
            Manifold.PointCount = 0;
        }
        else
        {
            Evaluate(ref Manifold, ref body._xf, ref body2._xf);
            flag2 = Manifold.PointCount > 0;
            for (int i = 0; i < Manifold.PointCount; i++)
            {
                ManifoldPoint value = Manifold.Points[i];
                value.NormalImpulse = 0f;
                value.TangentImpulse = 0f;
                ContactID id = value.Id;
                for (int j = 0; j < oldManifold.PointCount; j++)
                {
                    ManifoldPoint manifoldPoint = oldManifold.Points[j];
                    if (manifoldPoint.Id.Key == id.Key)
                    {
                        value.NormalImpulse = manifoldPoint.NormalImpulse;
                        value.TangentImpulse = manifoldPoint.TangentImpulse;
                        break;
                    }
                }
                Manifold.Points[i] = value;
            }
            if (flag2 != isTouching)
            {
                body.Awake = true;
                body2.Awake = true;
            }
        }
        IsTouching = flag2;
        if (!isTouching)
        {
            if (flag2)
            {
                bool flag3 = true;
                bool flag4 = true;
                if (FixtureA.OnCollision != null)
                {
                    Delegate[] invocationList = FixtureA.OnCollision.GetInvocationList();
                    for (int k = 0; k < invocationList.Length; k++)
                    {
                        OnCollisionHandler onCollisionEventHandler = (OnCollisionHandler)invocationList[k];
                        flag3 = onCollisionEventHandler(FixtureA, FixtureB, this) && flag3;
                    }
                }
                if (FixtureB.OnCollision != null)
                {
                    Delegate[] invocationList2 = FixtureB.OnCollision.GetInvocationList();
                    for (int l = 0; l < invocationList2.Length; l++)
                    {
                        OnCollisionHandler onCollisionEventHandler2 = (OnCollisionHandler)invocationList2[l];
                        flag4 = onCollisionEventHandler2(FixtureB, FixtureA, this) && flag4;
                    }
                }
                Enabled = flag3 && flag4;
                if (flag3 && flag4 && contactManager.BeginContact != null)
                {
                    Enabled = contactManager.BeginContact(this);
                }
                if (!Enabled)
                {
                    IsTouching = false;
                }
            }
        }
        else if (!flag2)
        {
            if (FixtureA != null && FixtureA.OnSeparation != null)
            {
                FixtureA.OnSeparation(FixtureA, FixtureB);
            }
            if (FixtureB != null && FixtureB.OnSeparation != null)
            {
                FixtureB.OnSeparation(FixtureB, FixtureA);
            }
            contactManager.EndContact?.Invoke(this);
        }
        if (!flag && contactManager.PreSolve != null)
        {
            contactManager.PreSolve(this, ref oldManifold);
        }
    }

    private void Evaluate(ref Manifold manifold, ref Transform transformA, ref Transform transformB)
    {
        switch (_type)
        {
            case ContactType.Polygon:
                Collision.Collision.CollidePolygons(ref manifold, (PolygonShape)FixtureA.Shape, ref transformA, (PolygonShape)FixtureB.Shape, ref transformB);
                break;
            case ContactType.PolygonAndCircle:
                Collision.Collision.CollidePolygonAndCircle(ref manifold, (PolygonShape)FixtureA.Shape, ref transformA, (CircleShape)FixtureB.Shape, ref transformB);
                break;
            case ContactType.EdgeAndCircle:
                Collision.Collision.CollideEdgeAndCircle(ref manifold, (EdgeShape)FixtureA.Shape, ref transformA, (CircleShape)FixtureB.Shape, ref transformB);
                break;
            case ContactType.EdgeAndPolygon:
                Collision.Collision.CollideEdgeAndPolygon(ref manifold, (EdgeShape)FixtureA.Shape, ref transformA, (PolygonShape)FixtureB.Shape, ref transformB);
                break;
            case ContactType.ChainAndCircle:
                {
                    ChainShape chainShape2 = (ChainShape)FixtureA.Shape;
                    chainShape2.GetChildEdge(_edge, ChildIndexA);
                    Collision.Collision.CollideEdgeAndCircle(ref manifold, _edge, ref transformA, (CircleShape)FixtureB.Shape, ref transformB);
                    break;
                }
            case ContactType.ChainAndPolygon:
                {
                    ChainShape chainShape = (ChainShape)FixtureA.Shape;
                    chainShape.GetChildEdge(_edge, ChildIndexA);
                    Collision.Collision.CollideEdgeAndPolygon(ref manifold, _edge, ref transformA, (PolygonShape)FixtureB.Shape, ref transformB);
                    break;
                }
            case ContactType.Circle:
                Collision.Collision.CollideCircles(ref manifold, (CircleShape)FixtureA.Shape, ref transformA, (CircleShape)FixtureB.Shape, ref transformB);
                break;
            case ContactType.NotSupported:
            default:
                break;
        }
    }

    internal static Contact Create(Fixture fixtureA, int indexA, Fixture fixtureB, int indexB)
    {
        ShapeType shapeType = fixtureA.Shape.ShapeType;
        ShapeType shapeType2 = fixtureB.Shape.ShapeType;
        Queue<Contact> contactPool = fixtureA.Body._world._contactPool;
        Contact contact;
        if (contactPool.Count <= 0)
        {
            contact = ((shapeType < shapeType2 && (shapeType != ShapeType.Edge || shapeType2 != ShapeType.Polygon)) || (shapeType2 == ShapeType.Edge && shapeType == ShapeType.Polygon)) ? new Contact(fixtureB, indexB, fixtureA, indexA) : new Contact(fixtureA, indexA, fixtureB, indexB);
        }
        else
        {
            contact = contactPool.Dequeue();
            if ((shapeType >= shapeType2 || (shapeType == ShapeType.Edge && shapeType2 == ShapeType.Polygon)) && (shapeType2 != ShapeType.Edge || shapeType != ShapeType.Polygon))
            {
                contact.Reset(fixtureA, indexA, fixtureB, indexB);
            }
            else
            {
                contact.Reset(fixtureB, indexB, fixtureA, indexA);
            }
        }
        contact._type = _registers[(int)shapeType, (int)shapeType2];
        return contact;
    }

    internal void Destroy()
    {
        FixtureA.Body._world._contactPool.Enqueue(this);
        if (Manifold.PointCount > 0 && !FixtureA.IsSensor && !FixtureB.IsSensor)
        {
            FixtureA.Body.Awake = true;
            FixtureB.Body.Awake = true;
        }
        Reset(null, 0, null, 0);
    }
}
