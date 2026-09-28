using System;
using System.Collections.Generic;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics;

public class BreakableBody
{
	private float[] _angularVelocitiesCache = new float[8];

	private bool _break;

	private Vector2[] _velocitiesCache = new Vector2[8];

	private World _world;

	public bool Broken;

	public Body MainBody;

	public List<Fixture> Parts = new List<Fixture>(8);

	public float Strength = 500f;

	public BreakableBody(IEnumerable<Vertices> vertices, World world, float density)
	{
		_world = world;
		ContactManager contactManager = _world.ContactManager;
		contactManager.PostSolve = (PostSolveDelegate)Delegate.Combine(contactManager.PostSolve, new PostSolveDelegate(PostSolve));
		MainBody = new Body(_world);
		MainBody.BodyType = BodyType.Dynamic;
		foreach (Vertices vertex in vertices)
		{
			PolygonShape shape = new PolygonShape(vertex, density);
			Fixture item = MainBody.CreateFixture(shape);
			Parts.Add(item);
		}
	}

	public BreakableBody(IEnumerable<Shape> shapes, World world)
	{
		_world = world;
		ContactManager contactManager = _world.ContactManager;
		contactManager.PostSolve = (PostSolveDelegate)Delegate.Combine(contactManager.PostSolve, new PostSolveDelegate(PostSolve));
		MainBody = new Body(_world);
		MainBody.BodyType = BodyType.Dynamic;
		foreach (Shape shape in shapes)
		{
			Fixture item = MainBody.CreateFixture(shape);
			Parts.Add(item);
		}
	}

	private void PostSolve(Contact contact, ContactVelocityConstraint impulse)
	{
		if (!Broken && (Parts.Contains(contact.FixtureA) || Parts.Contains(contact.FixtureB)))
		{
			float num = 0f;
			int pointCount = contact.Manifold.PointCount;
			for (int i = 0; i < pointCount; i++)
			{
				num = Math.Max(num, impulse.points[i].normalImpulse);
			}
			if (num > Strength)
			{
				_break = true;
			}
		}
	}

	public void Update()
	{
		if (_break)
		{
			Decompose();
			Broken = true;
			_break = false;
		}
		if (!Broken)
		{
			if (Parts.Count > _angularVelocitiesCache.Length)
			{
				_velocitiesCache = new Vector2[Parts.Count];
				_angularVelocitiesCache = new float[Parts.Count];
			}
			for (int i = 0; i < Parts.Count; i++)
			{
				ref Vector2 reference = ref _velocitiesCache[i];
				reference = Parts[i].Body.LinearVelocity;
				_angularVelocitiesCache[i] = Parts[i].Body.AngularVelocity;
			}
		}
	}

	private void Decompose()
	{
		ContactManager contactManager = _world.ContactManager;
		contactManager.PostSolve = (PostSolveDelegate)Delegate.Remove(contactManager.PostSolve, new PostSolveDelegate(PostSolve));
		for (int i = 0; i < Parts.Count; i++)
		{
			Fixture fixture = Parts[i];
			Shape shape = fixture.Shape.Clone();
			object userData = fixture.UserData;
			MainBody.DestroyFixture(fixture);
			Body body = BodyFactory.CreateBody(_world);
			body.BodyType = BodyType.Dynamic;
			body.Position = MainBody.Position;
			body.Rotation = MainBody.Rotation;
			body.UserData = MainBody.UserData;
			Fixture fixture2 = body.CreateFixture(shape);
			fixture2.UserData = userData;
			Parts[i] = fixture2;
			body.AngularVelocity = _angularVelocitiesCache[i];
			body.LinearVelocity = _velocitiesCache[i];
		}
		_world.RemoveBody(MainBody);
		_world.RemoveBreakableBody(this);
	}

	public void Break()
	{
		_break = true;
	}
}
