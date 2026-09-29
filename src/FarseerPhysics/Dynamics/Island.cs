using System;
using System.Diagnostics;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics;

public class Island
{
    private ContactManager _contactManager;

    private readonly ContactSolver _contactSolver = new();

    private Contact[] _contacts;

    private Joint[] _joints;

    private readonly Stopwatch _watch = new();

    public Body[] Bodies { get; set; }

    public int BodyCount { get; set; }

    public int ContactCount { get; set; }

    private int JointCount;

    private Velocity[] _velocities;

    private Position[] _positions;

    public int BodyCapacity { get; set; }

    public int ContactCapacity { get; set; }

    public void Reset(int bodyCapacity, int contactCapacity, int jointCapacity, ContactManager contactManager)
    {
        BodyCapacity = bodyCapacity;
        ContactCapacity = contactCapacity;
        BodyCount = 0;
        ContactCount = 0;
        JointCount = 0;
        _contactManager = contactManager;
        if (Bodies == null || Bodies.Length < bodyCapacity)
        {
            Bodies = new Body[bodyCapacity];
            _velocities = new Velocity[bodyCapacity];
            _positions = new Position[bodyCapacity];
        }
        if (_contacts == null || _contacts.Length < contactCapacity)
        {
            _contacts = new Contact[contactCapacity * 2];
        }
        if (_joints == null || _joints.Length < jointCapacity)
        {
            _joints = new Joint[jointCapacity * 2];
        }
    }

    public void Clear()
    {
        BodyCount = 0;
        ContactCount = 0;
        JointCount = 0;
    }

    public void Solve(ref TimeStep step, ref Vector2 gravity)
    {
        float dt = step.dt;
        for (int i = 0; i < BodyCount; i++)
        {
            Body body = Bodies[i];
            Vector2 c = body._sweep.C;
            float a = body._sweep.A;
            Vector2 linearVelocity = body._linearVelocity;
            float num = body._angularVelocity;
            body._sweep.C0 = body._sweep.C;
            body._sweep.A0 = body._sweep.A;
            if (body.BodyType == BodyType.Dynamic)
            {
                if (body.IgnoreGravity)
                {
                    linearVelocity += dt * (body._invMass * body._force);
                }
                else
                {
                    linearVelocity += dt * ((body.GravityScale * gravity) + (body._invMass * body._force));
                }
                num += dt * body._invI * body._torque;
                linearVelocity *= MathUtils.Clamp(1f - (dt * body.LinearDamping), 0f, 1f);
                num *= MathUtils.Clamp(1f - (dt * body.AngularDamping), 0f, 1f);
            }
            _positions[i].c = c;
            _positions[i].a = a;
            _velocities[i].v = linearVelocity;
            _velocities[i].w = num;
        }
        SolverData data = new()
        {
            step = step,
            positions = _positions,
            velocities = _velocities
        };
        _contactSolver.Reset(step, ContactCount, _contacts, _positions, _velocities);
        _contactSolver.InitializeVelocityConstraints();
        _contactSolver.WarmStart();
        _watch.Start();
        for (int j = 0; j < JointCount; j++)
        {
            if (_joints[j].Enabled)
            {
                _joints[j].InitVelocityConstraints(ref data);
            }
        }
        _watch.Stop();
        for (int k = 0; k < Settings.VelocityIterations; k++)
        {
            for (int l = 0; l < JointCount; l++)
            {
                Joint joint = _joints[l];
                if (joint.Enabled)
                {
                    _watch.Start();
                    joint.SolveVelocityConstraints(ref data);
                    joint.Validate(step.inv_dt);
                    _watch.Stop();
                }
            }
            _contactSolver.SolveVelocityConstraints();
        }
        _contactSolver.StoreImpulses();
        for (int m = 0; m < BodyCount; m++)
        {
            Vector2 c2 = _positions[m].c;
            float a2 = _positions[m].a;
            Vector2 v = _velocities[m].v;
            float num2 = _velocities[m].w;
            Vector2 vector = dt * v;
            if (Vector2.Dot(vector, vector) > 4f)
            {
                float num3 = 2f / vector.Length();
                v *= num3;
            }
            float num4 = dt * num2;
            if (num4 * num4 > 2.4674013f)
            {
                float num5 = (float)Math.PI / 2f / Math.Abs(num4);
                num2 *= num5;
            }
            c2 += dt * v;
            a2 += dt * num2;
            _positions[m].c = c2;
            _positions[m].a = a2;
            _velocities[m].v = v;
            _velocities[m].w = num2;
        }
        bool flag = false;
        for (int n = 0; n < Settings.PositionIterations; n++)
        {
            bool flag2 = _contactSolver.SolvePositionConstraints();
            bool flag3 = true;
            for (int num6 = 0; num6 < JointCount; num6++)
            {
                Joint joint2 = _joints[num6];
                if (joint2.Enabled)
                {
                    _watch.Start();
                    bool flag4 = joint2.SolvePositionConstraints(ref data);
                    _watch.Stop();
                    flag3 = flag3 && flag4;
                }
            }
            if (flag2 && flag3)
            {
                flag = true;
                break;
            }
        }
        _watch.Reset();
        for (int num7 = 0; num7 < BodyCount; num7++)
        {
            Body body2 = Bodies[num7];
            body2._sweep.C = _positions[num7].c;
            body2._sweep.A = _positions[num7].a;
            body2._linearVelocity = _velocities[num7].v;
            body2._angularVelocity = _velocities[num7].w;
            body2.SynchronizeTransform();
        }
        Report(_contactSolver.VelocityConstraints);
        if (!Settings.AllowSleep)
        {
            return;
        }
        float num8 = float.MaxValue;
        for (int num9 = 0; num9 < BodyCount; num9++)
        {
            Body body3 = Bodies[num9];
            if (body3.BodyType != BodyType.Static)
            {
                if (!body3.SleepingAllowed || body3._angularVelocity * body3._angularVelocity > 0.0012184697f || Vector2.Dot(body3._linearVelocity, body3._linearVelocity) > 0.0001f)
                {
                    body3._sleepTime = 0f;
                    num8 = 0f;
                }
                else
                {
                    body3._sleepTime += dt;
                    num8 = Math.Min(num8, body3._sleepTime);
                }
            }
        }
        if (num8 >= 0.5f && flag)
        {
            for (int num10 = 0; num10 < BodyCount; num10++)
            {
                Body body4 = Bodies[num10];
                body4.Awake = false;
            }
        }
    }

    internal void SolveTOI(ref TimeStep subStep, int toiIndexA, int toiIndexB)
    {
        for (int i = 0; i < BodyCount; i++)
        {
            Body body = Bodies[i];
            _positions[i].c = body._sweep.C;
            _positions[i].a = body._sweep.A;
            _velocities[i].v = body._linearVelocity;
            _velocities[i].w = body._angularVelocity;
        }
        _contactSolver.Reset(subStep, ContactCount, _contacts, _positions, _velocities);
        for (int j = 0; j < Settings.TOIPositionIterations; j++)
        {
            if (_contactSolver.SolveTOIPositionConstraints(toiIndexA, toiIndexB))
            {
                break;
            }
        }
        Bodies[toiIndexA]._sweep.C0 = _positions[toiIndexA].c;
        Bodies[toiIndexA]._sweep.A0 = _positions[toiIndexA].a;
        Bodies[toiIndexB]._sweep.C0 = _positions[toiIndexB].c;
        Bodies[toiIndexB]._sweep.A0 = _positions[toiIndexB].a;
        _contactSolver.InitializeVelocityConstraints();
        for (int k = 0; k < Settings.TOIVelocityIterations; k++)
        {
            _contactSolver.SolveVelocityConstraints();
        }
        float dt = subStep.dt;
        for (int l = 0; l < BodyCount; l++)
        {
            Vector2 c = _positions[l].c;
            float a = _positions[l].a;
            Vector2 v = _velocities[l].v;
            float num = _velocities[l].w;
            Vector2 vector = dt * v;
            if (Vector2.Dot(vector, vector) > 4f)
            {
                float num2 = 2f / vector.Length();
                v *= num2;
            }
            float num3 = dt * num;
            if (num3 * num3 > 2.4674013f)
            {
                float num4 = (float)Math.PI / 2f / Math.Abs(num3);
                num *= num4;
            }
            c += dt * v;
            a += dt * num;
            _positions[l].c = c;
            _positions[l].a = a;
            _velocities[l].v = v;
            _velocities[l].w = num;
            Body body2 = Bodies[l];
            body2._sweep.C = c;
            body2._sweep.A = a;
            body2._linearVelocity = v;
            body2._angularVelocity = num;
            body2.SynchronizeTransform();
        }
        Report(_contactSolver.VelocityConstraints);
    }

    public void Add(Body body)
    {
        body.IslandIndex = BodyCount;
        Bodies[BodyCount++] = body;
    }

    public void Add(Contact contact)
    {
        _contacts[ContactCount++] = contact;
    }

    public void Add(Joint joint)
    {
        _joints[JointCount++] = joint;
    }

    private void Report(ContactVelocityConstraint[] constraints)
    {
        if (_contactManager == null)
        {
            return;
        }
        for (int i = 0; i < ContactCount; i++)
        {
            Contact contact = _contacts[i];
            contact.FixtureA.AfterCollision?.Invoke(contact.FixtureA, contact.FixtureB, contact, constraints[i]);
            contact.FixtureB.AfterCollision?.Invoke(contact.FixtureB, contact.FixtureA, contact, constraints[i]);
            _contactManager.PostSolve?.Invoke(contact, constraints[i]);
        }
    }
}
