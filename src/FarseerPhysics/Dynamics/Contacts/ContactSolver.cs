using System;

using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Contacts;

public class ContactSolver
{
    public static class WorldManifold
    {
        public static void Initialize(ref Manifold manifold, ref Transform xfA, float radiusA, ref Transform xfB, float radiusB, out Vector2 normal, out FixedArray2<Vector2> points)
        {
            normal = Vector2.Zero;
            points = default;
            if (manifold.PointCount == 0)
            {
                return;
            }
            switch (manifold.Type)
            {
                case ManifoldType.Circles:
                    {
                        normal = new Vector2(1f, 0f);
                        Vector2 vector9 = MathUtils.Mul(ref xfA, manifold.LocalPoint);
                        Vector2 vector10 = MathUtils.Mul(ref xfB, manifold.Points[0].LocalPoint);
                        if (Vector2.DistanceSquared(vector9, vector10) > 1.4210855E-14f)
                        {
                            normal = vector10 - vector9;
                            normal.Normalize();
                        }
                        Vector2 vector11 = vector9 + (radiusA * normal);
                        Vector2 vector12 = vector10 - (radiusB * normal);
                        points[0] = 0.5f * (vector11 + vector12);
                        break;
                    }
                case ManifoldType.FaceA:
                    {
                        normal = MathUtils.Mul(xfA.q, manifold.LocalNormal);
                        Vector2 vector5 = MathUtils.Mul(ref xfA, manifold.LocalPoint);
                        for (int j = 0; j < manifold.PointCount; j++)
                        {
                            Vector2 vector6 = MathUtils.Mul(ref xfB, manifold.Points[j].LocalPoint);
                            Vector2 vector7 = vector6 + ((radiusA - Vector2.Dot(vector6 - vector5, normal)) * normal);
                            Vector2 vector8 = vector6 - (radiusB * normal);
                            points[j] = 0.5f * (vector7 + vector8);
                        }
                        break;
                    }
                case ManifoldType.FaceB:
                    {
                        normal = MathUtils.Mul(xfB.q, manifold.LocalNormal);
                        Vector2 vector = MathUtils.Mul(ref xfB, manifold.LocalPoint);
                        for (int i = 0; i < manifold.PointCount; i++)
                        {
                            Vector2 vector2 = MathUtils.Mul(ref xfA, manifold.Points[i].LocalPoint);
                            Vector2 vector3 = vector2 + ((radiusB - Vector2.Dot(vector2 - vector, normal)) * normal);
                            Vector2 vector4 = vector2 - (radiusA * normal);
                            points[i] = 0.5f * (vector4 + vector3);
                        }
                        normal = -normal;
                        break;
                    }
                default:
                    break;
            }
        }
    }

    private static class PositionSolverManifold
    {
        public static void Initialize(ContactPositionConstraint pc, Transform xfA, Transform xfB, int index, out Vector2 normal, out Vector2 point, out float separation)
        {
            switch (pc.Type)
            {
                case ManifoldType.Circles:
                    {
                        Vector2 vector5 = MathUtils.Mul(ref xfA, pc.LocalPoint);
                        Vector2 vector6 = MathUtils.Mul(ref xfB, pc.LocalPoints[0]);
                        normal = vector6 - vector5;
                        normal.Normalize();
                        point = 0.5f * (vector5 + vector6);
                        separation = Vector2.Dot(vector6 - vector5, normal) - pc.RadiusA - pc.RadiusB;
                        break;
                    }
                case ManifoldType.FaceA:
                    {
                        normal = MathUtils.Mul(xfA.q, pc.LocalNormal);
                        Vector2 vector3 = MathUtils.Mul(ref xfA, pc.LocalPoint);
                        Vector2 vector4 = MathUtils.Mul(ref xfB, pc.LocalPoints[index]);
                        separation = Vector2.Dot(vector4 - vector3, normal) - pc.RadiusA - pc.RadiusB;
                        point = vector4;
                        break;
                    }
                case ManifoldType.FaceB:
                    {
                        normal = MathUtils.Mul(xfB.q, pc.LocalNormal);
                        Vector2 vector = MathUtils.Mul(ref xfB, pc.LocalPoint);
                        Vector2 vector2 = MathUtils.Mul(ref xfA, pc.LocalPoints[index]);
                        separation = Vector2.Dot(vector2 - vector, normal) - pc.RadiusA - pc.RadiusB;
                        point = vector2;
                        normal = -normal;
                        break;
                    }
                default:
                    normal = Vector2.Zero;
                    point = Vector2.Zero;
                    separation = 0f;
                    break;
            }
        }
    }

    private TimeStep _step;

    private Position[] _positions;

    private Velocity[] _velocities;

    private ContactPositionConstraint[] _positionConstraints;

    public ContactVelocityConstraint[] VelocityConstraints { get; set; }

    private Contact[] _contacts;

    private int _count;

    public void Reset(TimeStep step, int count, Contact[] contacts, Position[] positions, Velocity[] velocities)
    {
        _step = step;
        _count = count;
        _positions = positions;
        _velocities = velocities;
        _contacts = contacts;
        if (VelocityConstraints == null || VelocityConstraints.Length < count)
        {
            VelocityConstraints = new ContactVelocityConstraint[count * 2];
            _positionConstraints = new ContactPositionConstraint[count * 2];
            for (int i = 0; i < VelocityConstraints.Length; i++)
            {
                VelocityConstraints[i] = new ContactVelocityConstraint();
            }
            for (int j = 0; j < _positionConstraints.Length; j++)
            {
                _positionConstraints[j] = new ContactPositionConstraint();
            }
        }
        for (int k = 0; k < _count; k++)
        {
            Contact contact = contacts[k];
            Fixture fixtureA = contact.FixtureA;
            Fixture fixtureB = contact.FixtureB;
            Shape shape = fixtureA.Shape;
            Shape shape2 = fixtureB.Shape;
            float radius = shape.Radius;
            float radius2 = shape2.Radius;
            Body body = fixtureA.Body;
            Body body2 = fixtureB.Body;
            Manifold manifold = contact.Manifold;
            int pointCount = manifold.PointCount;
            ContactVelocityConstraint contactVelocityConstraint = VelocityConstraints[k];
            contactVelocityConstraint.Friction = contact.Friction;
            contactVelocityConstraint.Restitution = contact.Restitution;
            contactVelocityConstraint.TangentSpeed = contact.TangentSpeed;
            contactVelocityConstraint.IndexA = body.IslandIndex;
            contactVelocityConstraint.IndexB = body2.IslandIndex;
            contactVelocityConstraint.InvMassA = body._invMass;
            contactVelocityConstraint.InvMassB = body2._invMass;
            contactVelocityConstraint.InvIA = body._invI;
            contactVelocityConstraint.InvIB = body2._invI;
            contactVelocityConstraint.ContactIndex = k;
            contactVelocityConstraint.PointCount = pointCount;
            contactVelocityConstraint.K.SetZero();
            contactVelocityConstraint.NormalMass.SetZero();
            ContactPositionConstraint contactPositionConstraint = _positionConstraints[k];
            contactPositionConstraint.IndexA = body.IslandIndex;
            contactPositionConstraint.IndexB = body2.IslandIndex;
            contactPositionConstraint.InvMassA = body._invMass;
            contactPositionConstraint.InvMassB = body2._invMass;
            contactPositionConstraint.LocalCenterA = body._sweep.LocalCenter;
            contactPositionConstraint.LocalCenterB = body2._sweep.LocalCenter;
            contactPositionConstraint.InvIA = body._invI;
            contactPositionConstraint.InvIB = body2._invI;
            contactPositionConstraint.LocalNormal = manifold.LocalNormal;
            contactPositionConstraint.LocalPoint = manifold.LocalPoint;
            contactPositionConstraint.PointCount = pointCount;
            contactPositionConstraint.RadiusA = radius;
            contactPositionConstraint.RadiusB = radius2;
            contactPositionConstraint.Type = manifold.Type;
            for (int l = 0; l < pointCount; l++)
            {
                ManifoldPoint manifoldPoint = manifold.Points[l];
                VelocityConstraintPoint velocityConstraintPoint = contactVelocityConstraint.Points[l];
                velocityConstraintPoint.NormalImpulse = _step.dtRatio * manifoldPoint.NormalImpulse;
                velocityConstraintPoint.TangentImpulse = _step.dtRatio * manifoldPoint.TangentImpulse;
                velocityConstraintPoint.RA = Vector2.Zero;
                velocityConstraintPoint.RB = Vector2.Zero;
                velocityConstraintPoint.NormalMass = 0f;
                velocityConstraintPoint.TangentMass = 0f;
                velocityConstraintPoint.VelocityBias = 0f;
                ref Vector2 reference = ref contactPositionConstraint.LocalPoints[l];
                reference = manifoldPoint.LocalPoint;
            }
        }
    }

    public void InitializeVelocityConstraints()
    {
        for (int i = 0; i < _count; i++)
        {
            ContactVelocityConstraint contactVelocityConstraint = VelocityConstraints[i];
            ContactPositionConstraint contactPositionConstraint = _positionConstraints[i];
            float radiusA = contactPositionConstraint.RadiusA;
            float radiusB = contactPositionConstraint.RadiusB;
            Manifold manifold = _contacts[contactVelocityConstraint.ContactIndex].Manifold;
            int indexA = contactVelocityConstraint.IndexA;
            int indexB = contactVelocityConstraint.IndexB;
            float invMassA = contactVelocityConstraint.InvMassA;
            float invMassB = contactVelocityConstraint.InvMassB;
            float invIA = contactVelocityConstraint.InvIA;
            float invIB = contactVelocityConstraint.InvIB;
            Vector2 localCenterA = contactPositionConstraint.LocalCenterA;
            Vector2 localCenterB = contactPositionConstraint.LocalCenterB;
            Vector2 c = _positions[indexA].c;
            float a = _positions[indexA].a;
            Vector2 v = _velocities[indexA].v;
            float w = _velocities[indexA].w;
            Vector2 c2 = _positions[indexB].c;
            float a2 = _positions[indexB].a;
            Vector2 v2 = _velocities[indexB].v;
            float w2 = _velocities[indexB].w;
            Transform xfA = default;
            Transform xfB = default;
            xfA.q.Set(a);
            xfB.q.Set(a2);
            xfA.p = c - MathUtils.Mul(xfA.q, localCenterA);
            xfB.p = c2 - MathUtils.Mul(xfB.q, localCenterB);
            WorldManifold.Initialize(ref manifold, ref xfA, radiusA, ref xfB, radiusB, out Vector2 normal, out FixedArray2<Vector2> points);
            contactVelocityConstraint.Normal = normal;
            int pointCount = contactVelocityConstraint.PointCount;
            for (int j = 0; j < pointCount; j++)
            {
                VelocityConstraintPoint velocityConstraintPoint = contactVelocityConstraint.Points[j];
                velocityConstraintPoint.RA = points[j] - c;
                velocityConstraintPoint.RB = points[j] - c2;
                float num = MathUtils.Cross(velocityConstraintPoint.RA, contactVelocityConstraint.Normal);
                float num2 = MathUtils.Cross(velocityConstraintPoint.RB, contactVelocityConstraint.Normal);
                float num3 = invMassA + invMassB + (invIA * num * num) + (invIB * num2 * num2);
                velocityConstraintPoint.NormalMass = (num3 > 0f) ? (1f / num3) : 0f;
                Vector2 b = MathUtils.Cross(contactVelocityConstraint.Normal, 1f);
                float num4 = MathUtils.Cross(velocityConstraintPoint.RA, b);
                float num5 = MathUtils.Cross(velocityConstraintPoint.RB, b);
                float num6 = invMassA + invMassB + (invIA * num4 * num4) + (invIB * num5 * num5);
                velocityConstraintPoint.TangentMass = (num6 > 0f) ? (1f / num6) : 0f;
                velocityConstraintPoint.VelocityBias = 0f;
                float num7 = Vector2.Dot(contactVelocityConstraint.Normal, v2 + MathUtils.Cross(w2, velocityConstraintPoint.RB) - v - MathUtils.Cross(w, velocityConstraintPoint.RA));
                if (num7 < -1f)
                {
                    velocityConstraintPoint.VelocityBias = (0f - contactVelocityConstraint.Restitution) * num7;
                }
            }
            if (contactVelocityConstraint.PointCount == 2)
            {
                VelocityConstraintPoint velocityConstraintPoint2 = contactVelocityConstraint.Points[0];
                VelocityConstraintPoint velocityConstraintPoint3 = contactVelocityConstraint.Points[1];
                float num8 = MathUtils.Cross(velocityConstraintPoint2.RA, contactVelocityConstraint.Normal);
                float num9 = MathUtils.Cross(velocityConstraintPoint2.RB, contactVelocityConstraint.Normal);
                float num10 = MathUtils.Cross(velocityConstraintPoint3.RA, contactVelocityConstraint.Normal);
                float num11 = MathUtils.Cross(velocityConstraintPoint3.RB, contactVelocityConstraint.Normal);
                float num12 = invMassA + invMassB + (invIA * num8 * num8) + (invIB * num9 * num9);
                float num13 = invMassA + invMassB + (invIA * num10 * num10) + (invIB * num11 * num11);
                float num14 = invMassA + invMassB + (invIA * num8 * num10) + (invIB * num9 * num11);
                if (num12 * num12 < 1000f * ((num12 * num13) - (num14 * num14)))
                {
                    contactVelocityConstraint.K.ex = new Vector2(num12, num14);
                    contactVelocityConstraint.K.ey = new Vector2(num14, num13);
                    contactVelocityConstraint.NormalMass = contactVelocityConstraint.K.Inverse;
                }
                else
                {
                    contactVelocityConstraint.PointCount = 1;
                }
            }
        }
    }

    public void WarmStart()
    {
        for (int i = 0; i < _count; i++)
        {
            ContactVelocityConstraint contactVelocityConstraint = VelocityConstraints[i];
            int indexA = contactVelocityConstraint.IndexA;
            int indexB = contactVelocityConstraint.IndexB;
            float invMassA = contactVelocityConstraint.InvMassA;
            float invIA = contactVelocityConstraint.InvIA;
            float invMassB = contactVelocityConstraint.InvMassB;
            float invIB = contactVelocityConstraint.InvIB;
            int pointCount = contactVelocityConstraint.PointCount;
            Vector2 v = _velocities[indexA].v;
            float num = _velocities[indexA].w;
            Vector2 v2 = _velocities[indexB].v;
            float num2 = _velocities[indexB].w;
            Vector2 normal = contactVelocityConstraint.Normal;
            Vector2 vector = MathUtils.Cross(normal, 1f);
            for (int j = 0; j < pointCount; j++)
            {
                VelocityConstraintPoint velocityConstraintPoint = contactVelocityConstraint.Points[j];
                Vector2 vector2 = (velocityConstraintPoint.NormalImpulse * normal) + (velocityConstraintPoint.TangentImpulse * vector);
                num -= invIA * MathUtils.Cross(velocityConstraintPoint.RA, vector2);
                v -= invMassA * vector2;
                num2 += invIB * MathUtils.Cross(velocityConstraintPoint.RB, vector2);
                v2 += invMassB * vector2;
            }
            _velocities[indexA].v = v;
            _velocities[indexA].w = num;
            _velocities[indexB].v = v2;
            _velocities[indexB].w = num2;
        }
    }

    public void SolveVelocityConstraints()
    {
        for (int i = 0; i < _count; i++)
        {
            ContactVelocityConstraint contactVelocityConstraint = VelocityConstraints[i];
            int indexA = contactVelocityConstraint.IndexA;
            int indexB = contactVelocityConstraint.IndexB;
            float invMassA = contactVelocityConstraint.InvMassA;
            float invIA = contactVelocityConstraint.InvIA;
            float invMassB = contactVelocityConstraint.InvMassB;
            float invIB = contactVelocityConstraint.InvIB;
            int pointCount = contactVelocityConstraint.PointCount;
            Vector2 v = _velocities[indexA].v;
            float num = _velocities[indexA].w;
            Vector2 v2 = _velocities[indexB].v;
            float num2 = _velocities[indexB].w;
            Vector2 normal = contactVelocityConstraint.Normal;
            Vector2 vector = MathUtils.Cross(normal, 1f);
            float friction = contactVelocityConstraint.Friction;
            for (int j = 0; j < pointCount; j++)
            {
                VelocityConstraintPoint velocityConstraintPoint = contactVelocityConstraint.Points[j];
                Vector2 value = v2 + MathUtils.Cross(num2, velocityConstraintPoint.RB) - v - MathUtils.Cross(num, velocityConstraintPoint.RA);
                float num3 = Vector2.Dot(value, vector) - contactVelocityConstraint.TangentSpeed;
                float num4 = velocityConstraintPoint.TangentMass * (0f - num3);
                float num5 = friction * velocityConstraintPoint.NormalImpulse;
                float num6 = MathUtils.Clamp(velocityConstraintPoint.TangentImpulse + num4, 0f - num5, num5);
                num4 = num6 - velocityConstraintPoint.TangentImpulse;
                velocityConstraintPoint.TangentImpulse = num6;
                Vector2 vector2 = num4 * vector;
                v -= invMassA * vector2;
                num -= invIA * MathUtils.Cross(velocityConstraintPoint.RA, vector2);
                v2 += invMassB * vector2;
                num2 += invIB * MathUtils.Cross(velocityConstraintPoint.RB, vector2);
            }
            if (contactVelocityConstraint.PointCount == 1)
            {
                VelocityConstraintPoint velocityConstraintPoint2 = contactVelocityConstraint.Points[0];
                Vector2 value2 = v2 + MathUtils.Cross(num2, velocityConstraintPoint2.RB) - v - MathUtils.Cross(num, velocityConstraintPoint2.RA);
                float num7 = Vector2.Dot(value2, normal);
                float num8 = (0f - velocityConstraintPoint2.NormalMass) * (num7 - velocityConstraintPoint2.VelocityBias);
                float num9 = Math.Max(velocityConstraintPoint2.NormalImpulse + num8, 0f);
                num8 = num9 - velocityConstraintPoint2.NormalImpulse;
                velocityConstraintPoint2.NormalImpulse = num9;
                Vector2 vector3 = num8 * normal;
                v -= invMassA * vector3;
                num -= invIA * MathUtils.Cross(velocityConstraintPoint2.RA, vector3);
                v2 += invMassB * vector3;
                num2 += invIB * MathUtils.Cross(velocityConstraintPoint2.RB, vector3);
            }
            else
            {
                VelocityConstraintPoint velocityConstraintPoint3 = contactVelocityConstraint.Points[0];
                VelocityConstraintPoint velocityConstraintPoint4 = contactVelocityConstraint.Points[1];
                Vector2 vector4 = new(velocityConstraintPoint3.NormalImpulse, velocityConstraintPoint4.NormalImpulse);
                Vector2 value3 = v2 + MathUtils.Cross(num2, velocityConstraintPoint3.RB) - v - MathUtils.Cross(num, velocityConstraintPoint3.RA);
                Vector2 value4 = v2 + MathUtils.Cross(num2, velocityConstraintPoint4.RB) - v - MathUtils.Cross(num, velocityConstraintPoint4.RA);
                float num10 = Vector2.Dot(value3, normal);
                float num11 = Vector2.Dot(value4, normal);
                Vector2 v3 = new()
                {
                    X = num10 - velocityConstraintPoint3.VelocityBias,
                    Y = num11 - velocityConstraintPoint4.VelocityBias
                };
                v3 -= MathUtils.Mul(ref contactVelocityConstraint.K, vector4);
                Vector2 vector5 = -MathUtils.Mul(ref contactVelocityConstraint.NormalMass, v3);
                if (vector5.X >= 0f && vector5.Y >= 0f)
                {
                    Vector2 vector6 = vector5 - vector4;
                    Vector2 vector7 = vector6.X * normal;
                    Vector2 vector8 = vector6.Y * normal;
                    v -= invMassA * (vector7 + vector8);
                    num -= invIA * (MathUtils.Cross(velocityConstraintPoint3.RA, vector7) + MathUtils.Cross(velocityConstraintPoint4.RA, vector8));
                    v2 += invMassB * (vector7 + vector8);
                    num2 += invIB * (MathUtils.Cross(velocityConstraintPoint3.RB, vector7) + MathUtils.Cross(velocityConstraintPoint4.RB, vector8));
                    velocityConstraintPoint3.NormalImpulse = vector5.X;
                    velocityConstraintPoint4.NormalImpulse = vector5.Y;
                }
                else
                {
                    vector5.X = (0f - velocityConstraintPoint3.NormalMass) * v3.X;
                    vector5.Y = 0f;
                    num11 = (contactVelocityConstraint.K.ex.Y * vector5.X) + v3.Y;
                    if (vector5.X >= 0f && num11 >= 0f)
                    {
                        Vector2 vector9 = vector5 - vector4;
                        Vector2 vector10 = vector9.X * normal;
                        Vector2 vector11 = vector9.Y * normal;
                        v -= invMassA * (vector10 + vector11);
                        num -= invIA * (MathUtils.Cross(velocityConstraintPoint3.RA, vector10) + MathUtils.Cross(velocityConstraintPoint4.RA, vector11));
                        v2 += invMassB * (vector10 + vector11);
                        num2 += invIB * (MathUtils.Cross(velocityConstraintPoint3.RB, vector10) + MathUtils.Cross(velocityConstraintPoint4.RB, vector11));
                        velocityConstraintPoint3.NormalImpulse = vector5.X;
                        velocityConstraintPoint4.NormalImpulse = vector5.Y;
                    }
                    else
                    {
                        vector5.X = 0f;
                        vector5.Y = (0f - velocityConstraintPoint4.NormalMass) * v3.Y;
                        num10 = (contactVelocityConstraint.K.ey.X * vector5.Y) + v3.X;
                        if (vector5.Y >= 0f && num10 >= 0f)
                        {
                            Vector2 vector12 = vector5 - vector4;
                            Vector2 vector13 = vector12.X * normal;
                            Vector2 vector14 = vector12.Y * normal;
                            v -= invMassA * (vector13 + vector14);
                            num -= invIA * (MathUtils.Cross(velocityConstraintPoint3.RA, vector13) + MathUtils.Cross(velocityConstraintPoint4.RA, vector14));
                            v2 += invMassB * (vector13 + vector14);
                            num2 += invIB * (MathUtils.Cross(velocityConstraintPoint3.RB, vector13) + MathUtils.Cross(velocityConstraintPoint4.RB, vector14));
                            velocityConstraintPoint3.NormalImpulse = vector5.X;
                            velocityConstraintPoint4.NormalImpulse = vector5.Y;
                        }
                        else
                        {
                            vector5.X = 0f;
                            vector5.Y = 0f;
                            num10 = v3.X;
                            num11 = v3.Y;
                            if (num10 >= 0f && num11 >= 0f)
                            {
                                Vector2 vector15 = vector5 - vector4;
                                Vector2 vector16 = vector15.X * normal;
                                Vector2 vector17 = vector15.Y * normal;
                                v -= invMassA * (vector16 + vector17);
                                num -= invIA * (MathUtils.Cross(velocityConstraintPoint3.RA, vector16) + MathUtils.Cross(velocityConstraintPoint4.RA, vector17));
                                v2 += invMassB * (vector16 + vector17);
                                num2 += invIB * (MathUtils.Cross(velocityConstraintPoint3.RB, vector16) + MathUtils.Cross(velocityConstraintPoint4.RB, vector17));
                                velocityConstraintPoint3.NormalImpulse = vector5.X;
                                velocityConstraintPoint4.NormalImpulse = vector5.Y;
                            }
                        }
                    }
                }
            }
            _velocities[indexA].v = v;
            _velocities[indexA].w = num;
            _velocities[indexB].v = v2;
            _velocities[indexB].w = num2;
        }
    }

    public void StoreImpulses()
    {
        for (int i = 0; i < _count; i++)
        {
            ContactVelocityConstraint contactVelocityConstraint = VelocityConstraints[i];
            Manifold manifold = _contacts[contactVelocityConstraint.ContactIndex].Manifold;
            for (int j = 0; j < contactVelocityConstraint.PointCount; j++)
            {
                ManifoldPoint value = manifold.Points[j];
                value.NormalImpulse = contactVelocityConstraint.Points[j].NormalImpulse;
                value.TangentImpulse = contactVelocityConstraint.Points[j].TangentImpulse;
                manifold.Points[j] = value;
            }
            _contacts[contactVelocityConstraint.ContactIndex].Manifold = manifold;
        }
    }

    public bool SolvePositionConstraints()
    {
        float num = 0f;
        for (int i = 0; i < _count; i++)
        {
            ContactPositionConstraint contactPositionConstraint = _positionConstraints[i];
            int indexA = contactPositionConstraint.IndexA;
            int indexB = contactPositionConstraint.IndexB;
            Vector2 localCenterA = contactPositionConstraint.LocalCenterA;
            float invMassA = contactPositionConstraint.InvMassA;
            float invIA = contactPositionConstraint.InvIA;
            Vector2 localCenterB = contactPositionConstraint.LocalCenterB;
            float invMassB = contactPositionConstraint.InvMassB;
            float invIB = contactPositionConstraint.InvIB;
            int pointCount = contactPositionConstraint.PointCount;
            Vector2 c = _positions[indexA].c;
            float num2 = _positions[indexA].a;
            Vector2 c2 = _positions[indexB].c;
            float num3 = _positions[indexB].a;
            for (int j = 0; j < pointCount; j++)
            {
                Transform xfA = default;
                Transform xfB = default;
                xfA.q.Set(num2);
                xfB.q.Set(num3);
                xfA.p = c - MathUtils.Mul(xfA.q, localCenterA);
                xfB.p = c2 - MathUtils.Mul(xfB.q, localCenterB);
                PositionSolverManifold.Initialize(contactPositionConstraint, xfA, xfB, j, out Vector2 normal, out Vector2 point, out float separation);
                Vector2 a = point - c;
                Vector2 a2 = point - c2;
                num = Math.Min(num, separation);
                float num4 = MathUtils.Clamp(0.2f * (separation + 0.005f), -0.2f, 0f);
                float num5 = MathUtils.Cross(a, normal);
                float num6 = MathUtils.Cross(a2, normal);
                float num7 = invMassA + invMassB + (invIA * num5 * num5) + (invIB * num6 * num6);
                float num8 = (num7 > 0f) ? ((0f - num4) / num7) : 0f;
                Vector2 vector = num8 * normal;
                c -= invMassA * vector;
                num2 -= invIA * MathUtils.Cross(a, vector);
                c2 += invMassB * vector;
                num3 += invIB * MathUtils.Cross(a2, vector);
            }
            _positions[indexA].c = c;
            _positions[indexA].a = num2;
            _positions[indexB].c = c2;
            _positions[indexB].a = num3;
        }
        return num >= -0.015f;
    }

    public bool SolveTOIPositionConstraints(int toiIndexA, int toiIndexB)
    {
        float num = 0f;
        for (int i = 0; i < _count; i++)
        {
            ContactPositionConstraint contactPositionConstraint = _positionConstraints[i];
            int indexA = contactPositionConstraint.IndexA;
            int indexB = contactPositionConstraint.IndexB;
            Vector2 localCenterA = contactPositionConstraint.LocalCenterA;
            Vector2 localCenterB = contactPositionConstraint.LocalCenterB;
            int pointCount = contactPositionConstraint.PointCount;
            float num2 = 0f;
            float num3 = 0f;
            if (indexA == toiIndexA || indexA == toiIndexB)
            {
                num2 = contactPositionConstraint.InvMassA;
                num3 = contactPositionConstraint.InvIA;
            }
            float num4 = 0f;
            float num5 = 0f;
            if (indexB == toiIndexA || indexB == toiIndexB)
            {
                num4 = contactPositionConstraint.InvMassB;
                num5 = contactPositionConstraint.InvIB;
            }
            Vector2 c = _positions[indexA].c;
            float num6 = _positions[indexA].a;
            Vector2 c2 = _positions[indexB].c;
            float num7 = _positions[indexB].a;
            for (int j = 0; j < pointCount; j++)
            {
                Transform xfA = default;
                Transform xfB = default;
                xfA.q.Set(num6);
                xfB.q.Set(num7);
                xfA.p = c - MathUtils.Mul(xfA.q, localCenterA);
                xfB.p = c2 - MathUtils.Mul(xfB.q, localCenterB);
                PositionSolverManifold.Initialize(contactPositionConstraint, xfA, xfB, j, out Vector2 normal, out Vector2 point, out float separation);
                Vector2 a = point - c;
                Vector2 a2 = point - c2;
                num = Math.Min(num, separation);
                float num8 = MathUtils.Clamp(0.2f * (separation + 0.005f), -0.2f, 0f);
                float num9 = MathUtils.Cross(a, normal);
                float num10 = MathUtils.Cross(a2, normal);
                float num11 = num2 + num4 + (num3 * num9 * num9) + (num5 * num10 * num10);
                float num12 = (num11 > 0f) ? ((0f - num8) / num11) : 0f;
                Vector2 vector = num12 * normal;
                c -= num2 * vector;
                num6 -= num3 * MathUtils.Cross(a, vector);
                c2 += num4 * vector;
                num7 += num5 * MathUtils.Cross(a2, vector);
            }
            _positions[indexA].c = c;
            _positions[indexA].a = num6;
            _positions[indexB].c = c2;
            _positions[indexB].a = num7;
        }
        return num >= -0.0075f;
    }
}
