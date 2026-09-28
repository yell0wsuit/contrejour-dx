using System;
using System.Collections.Generic;
using System.Diagnostics;

using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics;

public class World
{
    private float _invDt0;

    private Body[] _stack = new Body[64];

    private bool _stepComplete;

    private readonly HashSet<Body> _bodyAddList = [];

    private readonly HashSet<Body> _bodyRemoveList = [];

    private readonly HashSet<Joint> _jointAddList = [];

    private readonly HashSet<Joint> _jointRemoveList = [];

    private Func<Fixture, bool> _queryAABBCallback;

    private readonly Func<int, bool> _queryAABBCallbackWrapper;

    private readonly TOIInput _input = new();

    private Fixture _myFixture;

    private Vector2 _point1;

    private Vector2 _point2;

    private List<Fixture> _testPointAllFixtures;

    private readonly Stopwatch _watch = new();

    private Func<Fixture, Vector2, Vector2, float, float> _rayCastCallback;

    private readonly Func<RayCastInput, int, float> _rayCastCallbackWrapper;

    internal Queue<Contact> _contactPool = new(256);

    internal bool _worldHasNewFixture;

    public BodyHandler BodyAdded;

    public BodyHandler BodyRemoved;

    public FixtureHandler FixtureAdded;

    public FixtureHandler FixtureRemoved;

    public JointHandler JointAdded;

    public JointHandler JointRemoved;

    public ControllerHandler ControllerAdded;

    public ControllerHandler ControllerRemoved;

    public Vector2 Gravity;

    public List<Controller> ControllerList { get; private set; }

    public List<BreakableBody> BreakableBodyList { get; private set; }

    public float UpdateTime { get; private set; }

    public float ContinuousPhysicsTime { get; private set; }

    public float ControllersUpdateTime { get; private set; }

    public float AddRemoveTime { get; private set; }

    public float NewContactsTime { get; private set; }

    public float ContactsUpdateTime { get; private set; }

    public float SolveUpdateTime { get; private set; }

    public int ProxyCount => ContactManager.BroadPhase.ProxyCount;

    public ContactManager ContactManager { get; private set; }

    public List<Body> BodyList { get; private set; }

    public List<Joint> JointList { get; private set; }

    public List<Contact> ContactList => ContactManager.ContactList;

    public bool Enabled { get; set; }

    public Island Island { get; private set; }

    public World(Vector2 gravity)
    {
        Island = new Island();
        Enabled = true;
        ControllerList = [];
        BreakableBodyList = [];
        BodyList = new List<Body>(32);
        JointList = new List<Joint>(32);
        _queryAABBCallbackWrapper = QueryAABBCallbackWrapper;
        _rayCastCallbackWrapper = RayCastCallbackWrapper;
        ContactManager = new ContactManager(new DynamicTreeBroadPhase());
        Gravity = gravity;
    }

    private void ProcessRemovedJoints()
    {
        if (_jointRemoveList.Count <= 0)
        {
            return;
        }
        foreach (Joint jointRemove in _jointRemoveList)
        {
            bool collideConnected = jointRemove.CollideConnected;
            _ = JointList.Remove(jointRemove);
            Body bodyA = jointRemove.BodyA;
            Body bodyB = jointRemove.BodyB;
            bodyA.Awake = true;
            if (!jointRemove.IsFixedType())
            {
                bodyB.Awake = true;
            }
            jointRemove.EdgeA.Prev?.Next = jointRemove.EdgeA.Next;
            jointRemove.EdgeA.Next?.Prev = jointRemove.EdgeA.Prev;
            if (jointRemove.EdgeA == bodyA.JointList)
            {
                bodyA.JointList = jointRemove.EdgeA.Next;
            }
            jointRemove.EdgeA.Prev = null;
            jointRemove.EdgeA.Next = null;
            if (!jointRemove.IsFixedType())
            {
                jointRemove.EdgeB.Prev?.Next = jointRemove.EdgeB.Next;
                jointRemove.EdgeB.Next?.Prev = jointRemove.EdgeB.Prev;
                if (jointRemove.EdgeB == bodyB.JointList)
                {
                    bodyB.JointList = jointRemove.EdgeB.Next;
                }
                jointRemove.EdgeB.Prev = null;
                jointRemove.EdgeB.Next = null;
            }
            if (!jointRemove.IsFixedType() && !collideConnected)
            {
                for (ContactEdge contactEdge = bodyB.ContactList; contactEdge != null; contactEdge = contactEdge.Next)
                {
                    if (contactEdge.Other == bodyA)
                    {
                        contactEdge.Contact.FilterFlag = true;
                    }
                }
            }
            JointRemoved?.Invoke(jointRemove);
        }
        _jointRemoveList.Clear();
    }

    private void ProcessAddedJoints()
    {
        if (_jointAddList.Count <= 0)
        {
            return;
        }
        foreach (Joint jointAdd in _jointAddList)
        {
            JointList.Add(jointAdd);
            jointAdd.EdgeA.Joint = jointAdd;
            jointAdd.EdgeA.Other = jointAdd.BodyB;
            jointAdd.EdgeA.Prev = null;
            jointAdd.EdgeA.Next = jointAdd.BodyA.JointList;
            jointAdd.BodyA.JointList?.Prev = jointAdd.EdgeA;
            jointAdd.BodyA.JointList = jointAdd.EdgeA;
            if (!jointAdd.IsFixedType())
            {
                jointAdd.EdgeB.Joint = jointAdd;
                jointAdd.EdgeB.Other = jointAdd.BodyA;
                jointAdd.EdgeB.Prev = null;
                jointAdd.EdgeB.Next = jointAdd.BodyB.JointList;
                jointAdd.BodyB.JointList?.Prev = jointAdd.EdgeB;
                jointAdd.BodyB.JointList = jointAdd.EdgeB;
                Body bodyA = jointAdd.BodyA;
                Body bodyB = jointAdd.BodyB;
                if (!jointAdd.CollideConnected)
                {
                    for (ContactEdge contactEdge = bodyB.ContactList; contactEdge != null; contactEdge = contactEdge.Next)
                    {
                        if (contactEdge.Other == bodyA)
                        {
                            contactEdge.Contact.FilterFlag = true;
                        }
                    }
                }
            }
            JointAdded?.Invoke(jointAdd);
        }
        _jointAddList.Clear();
    }

    private void ProcessAddedBodies()
    {
        if (_bodyAddList.Count <= 0)
        {
            return;
        }
        foreach (Body bodyAdd in _bodyAddList)
        {
            BodyList.Add(bodyAdd);
            BodyAdded?.Invoke(bodyAdd);
        }
        _bodyAddList.Clear();
    }

    private void ProcessRemovedBodies()
    {
        if (_bodyRemoveList.Count <= 0)
        {
            return;
        }
        foreach (Body bodyRemove in _bodyRemoveList)
        {
            JointEdge jointEdge = bodyRemove.JointList;
            while (jointEdge != null)
            {
                JointEdge jointEdge2 = jointEdge;
                jointEdge = jointEdge.Next;
                RemoveJoint(jointEdge2.Joint);
            }
            bodyRemove.JointList = null;
            ContactEdge contactEdge = bodyRemove.ContactList;
            while (contactEdge != null)
            {
                ContactEdge contactEdge2 = contactEdge;
                contactEdge = contactEdge.Next;
                ContactManager.Destroy(contactEdge2.Contact);
            }
            bodyRemove.ContactList = null;
            for (int i = 0; i < bodyRemove.FixtureList.Count; i++)
            {
                bodyRemove.FixtureList[i].DestroyProxies(ContactManager.BroadPhase);
                bodyRemove.FixtureList[i].Destroy();
            }
            bodyRemove.FixtureList = null;
            _ = BodyList.Remove(bodyRemove);
            BodyRemoved?.Invoke(bodyRemove);
        }
        _bodyRemoveList.Clear();
    }

    private bool QueryAABBCallbackWrapper(int proxyId)
    {
        FixtureProxy proxy = ContactManager.BroadPhase.GetProxy(proxyId);
        return _queryAABBCallback(proxy.Fixture);
    }

    private float RayCastCallbackWrapper(RayCastInput rayCastInput, int proxyId)
    {
        FixtureProxy proxy = ContactManager.BroadPhase.GetProxy(proxyId);
        Fixture fixture = proxy.Fixture;
        int childIndex = proxy.ChildIndex;
        if (fixture.RayCast(out RayCastOutput output, ref rayCastInput, childIndex))
        {
            float fraction = output.Fraction;
            Vector2 arg = ((1f - fraction) * rayCastInput.Point1) + (fraction * rayCastInput.Point2);
            return _rayCastCallback(fixture, arg, output.Normal, fraction);
        }
        return rayCastInput.MaxFraction;
    }

    private void Solve(ref TimeStep step)
    {
        Island.Reset(BodyList.Count, ContactManager.ContactList.Count, JointList.Count, ContactManager);
        foreach (Body body4 in BodyList)
        {
            body4._island = false;
        }
        foreach (Contact contact2 in ContactManager.ContactList)
        {
            contact2.IslandFlag = false;
        }
        foreach (Joint joint in JointList)
        {
            joint.IslandFlag = false;
        }
        int count = BodyList.Count;
        if (count > _stack.Length)
        {
            _stack = new Body[Math.Max(_stack.Length * 2, count)];
        }
        for (int num = BodyList.Count - 1; num >= 0; num--)
        {
            Body body = BodyList[num];
            if (!body._island && body.Awake && body.Enabled && body.BodyType != BodyType.Static)
            {
                Island.Clear();
                int num2 = 0;
                _stack[num2++] = body;
                body._island = true;
                while (num2 > 0)
                {
                    Body body2 = _stack[--num2];
                    Island.Add(body2);
                    body2.Awake = true;
                    if (body2.BodyType == BodyType.Static)
                    {
                        continue;
                    }
                    for (ContactEdge contactEdge = body2.ContactList; contactEdge != null; contactEdge = contactEdge.Next)
                    {
                        Contact contact = contactEdge.Contact;
                        if (!contact.IslandFlag && contactEdge.Contact.Enabled && contactEdge.Contact.IsTouching)
                        {
                            bool isSensor = contact.FixtureA.IsSensor;
                            bool isSensor2 = contact.FixtureB.IsSensor;
                            if (!isSensor && !isSensor2)
                            {
                                Island.Add(contact);
                                contact.IslandFlag = true;
                                Body other = contactEdge.Other;
                                if (!other._island)
                                {
                                    _stack[num2++] = other;
                                    other._island = true;
                                }
                            }
                        }
                    }
                    for (JointEdge jointEdge = body2.JointList; jointEdge != null; jointEdge = jointEdge.Next)
                    {
                        if (!jointEdge.Joint.IslandFlag)
                        {
                            Body other2 = jointEdge.Other;
                            if (other2 != null)
                            {
                                if (other2.Enabled)
                                {
                                    Island.Add(jointEdge.Joint);
                                    jointEdge.Joint.IslandFlag = true;
                                    if (!other2._island)
                                    {
                                        _stack[num2++] = other2;
                                        other2._island = true;
                                    }
                                }
                            }
                            else
                            {
                                Island.Add(jointEdge.Joint);
                                jointEdge.Joint.IslandFlag = true;
                            }
                        }
                    }
                }
                Island.Solve(ref step, ref Gravity);
                for (int i = 0; i < Island.BodyCount; i++)
                {
                    Body body3 = Island.Bodies[i];
                    if (body3.BodyType == BodyType.Static)
                    {
                        body3._island = false;
                    }
                }
            }
        }
        foreach (Body body5 in BodyList)
        {
            if (body5._island && body5.BodyType != BodyType.Static)
            {
                body5.SynchronizeFixtures();
            }
        }
        ContactManager.FindNewContacts();
    }

    private void SolveTOI(ref TimeStep step)
    {
        Island.Reset(64, 32, 0, ContactManager);
        if (_stepComplete)
        {
            for (int i = 0; i < BodyList.Count; i++)
            {
                BodyList[i]._island = false;
                BodyList[i]._sweep.Alpha0 = 0f;
            }
            for (int j = 0; j < ContactManager.ContactList.Count; j++)
            {
                Contact contact = ContactManager.ContactList[j];
                contact.IslandFlag = false;
                contact.TOIFlag = false;
                contact._toiCount = 0;
                contact._toi = 1f;
            }
        }
        TimeStep subStep = default;
        while (true)
        {
            Contact contact2 = null;
            float num = 1f;
            for (int k = 0; k < ContactManager.ContactList.Count; k++)
            {
                Contact contact3 = ContactManager.ContactList[k];
                if (!contact3.Enabled || contact3._toiCount > 8)
                {
                    continue;
                }
                float num2;
                if (contact3.TOIFlag)
                {
                    num2 = contact3._toi;
                }
                else
                {
                    Fixture fixtureA = contact3.FixtureA;
                    Fixture fixtureB = contact3.FixtureB;
                    if (fixtureA.IsSensor || fixtureB.IsSensor)
                    {
                        continue;
                    }
                    Body body = fixtureA.Body;
                    Body body2 = fixtureB.Body;
                    BodyType bodyType = body.BodyType;
                    BodyType bodyType2 = body2.BodyType;
                    bool flag = body.Awake && bodyType != BodyType.Static;
                    bool flag2 = body2.Awake && bodyType2 != BodyType.Static;
                    if (!flag && !flag2)
                    {
                        continue;
                    }
                    bool flag3 = (body.IsBullet || bodyType != BodyType.Dynamic) && (fixtureA.IgnoreCCDWith & fixtureB.CollisionCategories) == 0 && !body.IgnoreCCD;
                    bool flag4 = (body2.IsBullet || bodyType2 != BodyType.Dynamic) && (fixtureB.IgnoreCCDWith & fixtureA.CollisionCategories) == 0 && !body2.IgnoreCCD;
                    if (!flag3 && !flag4)
                    {
                        continue;
                    }
                    float alpha = body._sweep.Alpha0;
                    if (body._sweep.Alpha0 < body2._sweep.Alpha0)
                    {
                        alpha = body2._sweep.Alpha0;
                        body._sweep.Advance(alpha);
                    }
                    else if (body2._sweep.Alpha0 < body._sweep.Alpha0)
                    {
                        alpha = body._sweep.Alpha0;
                        body2._sweep.Advance(alpha);
                    }
                    _input.ProxyA.Set(fixtureA.Shape, contact3.ChildIndexA);
                    _input.ProxyB.Set(fixtureB.Shape, contact3.ChildIndexB);
                    _input.SweepA = body._sweep;
                    _input.SweepB = body2._sweep;
                    _input.TMax = 1f;
                    TimeOfImpact.CalculateTimeOfImpact(out TOIOutput output, _input);
                    float t = output.T;
                    num2 = contact3._toi = (output.State != TOIOutputState.Touching) ? 1f : Math.Min(alpha + ((1f - alpha) * t), 1f);
                    contact3.TOIFlag = true;
                }
                if (num2 < num)
                {
                    contact2 = contact3;
                    num = num2;
                }
            }
            if (contact2 == null || 0.9999988f < num)
            {
                break;
            }
            Fixture fixtureA2 = contact2.FixtureA;
            Fixture fixtureB2 = contact2.FixtureB;
            Body body3 = fixtureA2.Body;
            Body body4 = fixtureB2.Body;
            Sweep sweep = body3._sweep;
            Sweep sweep2 = body4._sweep;
            body3.Advance(num);
            body4.Advance(num);
            contact2.Update(ContactManager);
            contact2.TOIFlag = false;
            contact2._toiCount++;
            if (!contact2.Enabled || !contact2.IsTouching)
            {
                contact2.Enabled = false;
                body3._sweep = sweep;
                body4._sweep = sweep2;
                body3.SynchronizeTransform();
                body4.SynchronizeTransform();
                continue;
            }
            body3.Awake = true;
            body4.Awake = true;
            Island.Clear();
            Island.Add(body3);
            Island.Add(body4);
            Island.Add(contact2);
            body3._island = true;
            body4._island = true;
            contact2.IslandFlag = true;
            Body[] array = [body3, body4];
            for (int l = 0; l < 2; l++)
            {
                Body body5 = array[l];
                if (body5.BodyType != BodyType.Dynamic)
                {
                    continue;
                }
                for (ContactEdge contactEdge = body5.ContactList; contactEdge != null; contactEdge = contactEdge.Next)
                {
                    Contact contact4 = contactEdge.Contact;
                    if (Island.BodyCount == Island.BodyCapacity || Island.ContactCount == Island.ContactCapacity)
                    {
                        break;
                    }
                    if (!contact4.IslandFlag)
                    {
                        Body other = contactEdge.Other;
                        if ((other.BodyType != BodyType.Dynamic || body5.IsBullet || other.IsBullet) && !contact4.FixtureA.IsSensor && !contact4.FixtureB.IsSensor)
                        {
                            Sweep sweep3 = other._sweep;
                            if (!other._island)
                            {
                                other.Advance(num);
                            }
                            contact4.Update(ContactManager);
                            if (!contact4.Enabled)
                            {
                                other._sweep = sweep3;
                                other.SynchronizeTransform();
                            }
                            else if (!contact4.IsTouching)
                            {
                                other._sweep = sweep3;
                                other.SynchronizeTransform();
                            }
                            else
                            {
                                contact4.IslandFlag = true;
                                Island.Add(contact4);
                                if (!other._island)
                                {
                                    other._island = true;
                                    if (other.BodyType != BodyType.Static)
                                    {
                                        other.Awake = true;
                                    }
                                    Island.Add(other);
                                }
                            }
                        }
                    }
                }
            }
            subStep.dt = (1f - num) * step.dt;
            subStep.inv_dt = 1f / subStep.dt;
            subStep.dtRatio = 1f;
            Island.SolveTOI(ref subStep, body3.IslandIndex, body4.IslandIndex);
            for (int m = 0; m < Island.BodyCount; m++)
            {
                Body body6 = Island.Bodies[m];
                body6._island = false;
                if (body6.BodyType == BodyType.Dynamic)
                {
                    body6.SynchronizeFixtures();
                    for (ContactEdge contactEdge2 = body6.ContactList; contactEdge2 != null; contactEdge2 = contactEdge2.Next)
                    {
                        contactEdge2.Contact.TOIFlag = false;
                        contactEdge2.Contact.IslandFlag = false;
                    }
                }
            }
            ContactManager.FindNewContacts();
        }
        _stepComplete = true;
    }

    internal void AddBody(Body body)
    {
        if (!_bodyAddList.Contains(body))
        {
            _ = _bodyAddList.Add(body);
        }
    }

    public void RemoveBody(Body body)
    {
        if (!_bodyRemoveList.Contains(body))
        {
            _ = _bodyRemoveList.Add(body);
        }
    }

    public void AddJoint(Joint joint)
    {
        if (!_jointAddList.Contains(joint))
        {
            _ = _jointAddList.Add(joint);
        }
    }

    public void RemoveJoint(Joint joint)
    {
        if (!_jointRemoveList.Contains(joint))
        {
            _ = _jointRemoveList.Add(joint);
        }
    }

    public void ProcessChanges()
    {
        ProcessAddedBodies();
        ProcessAddedJoints();
        ProcessRemovedBodies();
        ProcessRemovedJoints();
    }

    public void Step(float dt)
    {
        if (Enabled)
        {
            _watch.Start();
            ProcessChanges();
            AddRemoveTime = _watch.ElapsedTicks;
            if (_worldHasNewFixture)
            {
                ContactManager.FindNewContacts();
                _worldHasNewFixture = false;
            }
            NewContactsTime = _watch.ElapsedTicks - AddRemoveTime;
            TimeStep step = default;
            step.inv_dt = (dt > 0f) ? (1f / dt) : 0f;
            step.dt = dt;
            step.dtRatio = _invDt0 * dt;
            for (int i = 0; i < ControllerList.Count; i++)
            {
                ControllerList[i].Update(dt);
            }
            ControllersUpdateTime = _watch.ElapsedTicks - (AddRemoveTime + NewContactsTime);
            ContactManager.Collide();
            ContactsUpdateTime = _watch.ElapsedTicks - (AddRemoveTime + NewContactsTime + ControllersUpdateTime);
            Solve(ref step);
            SolveUpdateTime = _watch.ElapsedTicks - (AddRemoveTime + NewContactsTime + ControllersUpdateTime + ContactsUpdateTime);
            if (Settings.ContinuousPhysics)
            {
                SolveTOI(ref step);
            }
            ContinuousPhysicsTime = _watch.ElapsedTicks - (AddRemoveTime + NewContactsTime + ControllersUpdateTime + ContactsUpdateTime + SolveUpdateTime);
            ClearForces();
            for (int j = 0; j < BreakableBodyList.Count; j++)
            {
                BreakableBodyList[j].Update();
            }
            _invDt0 = step.inv_dt;
            _watch.Stop();
            UpdateTime = _watch.ElapsedTicks;
            _watch.Reset();
        }
    }

    public void ClearForces()
    {
        for (int i = 0; i < BodyList.Count; i++)
        {
            Body body = BodyList[i];
            body._force = Vector2.Zero;
            body._torque = 0f;
        }
    }

    public void QueryAABB(Func<Fixture, bool> callback, ref AABB aabb)
    {
        _queryAABBCallback = callback;
        ContactManager.BroadPhase.Query(_queryAABBCallbackWrapper, ref aabb);
        _queryAABBCallback = null;
    }

    public List<Fixture> QueryAABB(ref AABB aabb)
    {
        List<Fixture> affected = [];
        QueryAABB(delegate (Fixture fixture)
        {
            affected.Add(fixture);
            return true;
        }, ref aabb);
        return affected;
    }

    public void RayCast(Func<Fixture, Vector2, Vector2, float, float> callback, Vector2 point1, Vector2 point2)
    {
        RayCastInput input = new()
        {
            MaxFraction = 1f,
            Point1 = point1,
            Point2 = point2
        };
        _rayCastCallback = callback;
        ContactManager.BroadPhase.RayCast(_rayCastCallbackWrapper, ref input);
        _rayCastCallback = null;
    }

    public List<Fixture> RayCast(Vector2 point1, Vector2 point2)
    {
        List<Fixture> affected = [];
        RayCast(delegate (Fixture f, Vector2 p, Vector2 n, float fr)
        {
            affected.Add(f);
            return 1f;
        }, point1, point2);
        return affected;
    }

    public void AddController(Controller controller)
    {
        controller.World = this;
        ControllerList.Add(controller);
        ControllerAdded?.Invoke(controller);
    }

    public void RemoveController(Controller controller)
    {
        if (ControllerList.Contains(controller))
        {
            _ = ControllerList.Remove(controller);
            ControllerRemoved?.Invoke(controller);
        }
    }

    public void AddBreakableBody(BreakableBody breakableBody)
    {
        BreakableBodyList.Add(breakableBody);
    }

    public void RemoveBreakableBody(BreakableBody breakableBody)
    {
        _ = BreakableBodyList.Remove(breakableBody);
    }

    public Fixture TestPoint(Vector2 point)
    {
        Vector2 vector = new(1.1920929E-07f, 1.1920929E-07f);
        AABB aabb = default;
        aabb.LowerBound = point - vector;
        aabb.UpperBound = point + vector;
        _myFixture = null;
        _point1 = point;
        QueryAABB(TestPointCallback, ref aabb);
        return _myFixture;
    }

    private bool TestPointCallback(Fixture fixture)
    {
        if (fixture.TestPoint(ref _point1))
        {
            _myFixture = fixture;
            return false;
        }
        return true;
    }

    public List<Fixture> TestPointAll(Vector2 point)
    {
        Vector2 vector = new(1.1920929E-07f, 1.1920929E-07f);
        AABB aabb = default;
        aabb.LowerBound = point - vector;
        aabb.UpperBound = point + vector;
        _point2 = point;
        _testPointAllFixtures = [];
        QueryAABB(TestPointAllCallback, ref aabb);
        return _testPointAllFixtures;
    }

    private bool TestPointAllCallback(Fixture fixture)
    {
        if (fixture.TestPoint(ref _point2))
        {
            _testPointAllFixtures.Add(fixture);
        }
        return true;
    }

    public void ShiftOrigin(Vector2 newOrigin)
    {
        foreach (Body body in BodyList)
        {
            body._xf.p -= newOrigin;
            body._sweep.C0 -= newOrigin;
            body._sweep.C -= newOrigin;
        }
        foreach (Joint joint in JointList)
        {
            _ = joint;
        }
        ContactManager.BroadPhase.ShiftOrigin(newOrigin);
    }

    public void Clear()
    {
        ProcessChanges();
        for (int num = BodyList.Count - 1; num >= 0; num--)
        {
            RemoveBody(BodyList[num]);
        }
        for (int num2 = ControllerList.Count - 1; num2 >= 0; num2--)
        {
            RemoveController(ControllerList[num2]);
        }
        for (int num3 = BreakableBodyList.Count - 1; num3 >= 0; num3--)
        {
            RemoveBreakableBody(BreakableBodyList[num3]);
        }
        ProcessChanges();
    }
}
