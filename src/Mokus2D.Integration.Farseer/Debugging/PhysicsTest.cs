using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Mokus2D.Integration.Farseer.Debugging
{
    public class PhysicsTest : Node
    {
        private readonly World world = new(new Vector2(0f, 10f));

        private readonly FarseerDebugNode debugNode;

        public PhysicsTest()
        {
            debugNode = new FarseerDebugNode(world, 30f);
            AddChild(debugNode);
            CreateCircle(new Vector2(1f, 0f));
            CreateRectangle(new Vector2(1f, 1f));
            CreateRectangle(new Vector2(2f, 2f));
            Body body = BodyFactory.CreateBody(world, Vector2.Zero);
            _ = FixtureFactory.AttachRectangle(10f, 2f, 0.5f, new Vector2(0f, 10f), body);
            _ = FixtureFactory.AttachRectangle(10f, 2f, 0.5f, Vector2.Zero, body);
            _ = FixtureFactory.AttachCircle(1f, 2f, body);
            body.BodyType = BodyType.Static;
        }

        private void CreateCircle(Vector2 position)
        {
            Body body = BodyFactory.CreateBody(world);
            body.BodyType = BodyType.Dynamic;
            body.IsSensor = false;
            Fixture fixture = FixtureFactory.AttachCircle(1f, 1f, body);
            fixture.Restitution = 0.3f;
            fixture.Friction = 0.5f;
            body.Position = position;
        }

        private void CreateRectangle(Vector2 position)
        {
            Body body = BodyFactory.CreateBody(world, position);
            body.BodyType = BodyType.Dynamic;
            Fixture fixture = FixtureFactory.AttachRectangle(2f, 2f, 0.5f, Vector2.Zero, body);
            fixture.Restitution = 0.3f;
            fixture.Friction = 0.5f;
        }

        public override void Update(float time)
        {
            base.Update(time);
            world.Step(time);
        }
    }
}
