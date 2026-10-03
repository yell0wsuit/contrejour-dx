using System.Numerics;
using System.Reflection;

using ContreJour.Gameplay;

using FarseerPhysics.Dynamics;

using Xunit;

namespace ContreJour.Tests
{
    public class BaloonTailTests
    {
        [Theory]
        [InlineData(1, -0.2332698176966212f)]
        [InlineData(2, 0.2286692069463435f)]
        [InlineData(3, 0.2240114286654847f)]
        public void SpringOscillatorUsesThePreviousFrameAmplitude(int frames, float expected)
        {
            World world = new(Vector2.Zero);
            Body body = world.CreateCircle(0.4f, Vector2.Zero, density: 1f, dynamic: true);
            BaloonTail tail = new(world, body);
            typeof(BaloonTail).GetField("phase", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(tail, 0f);
            tail.SpringTail(1f);
            for (int i = 0; i < frames; i++)
            {
                tail.Update(1f / 60f, 1f + ((i + 1) / 60f));
            }
            Vector2 fawn = (Vector2)typeof(BaloonTail).GetField("endFawn", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(tail);
            // Expected values were evaluated from cj.js CosChanger's source
            // formula, with phase zero and Y reflected into DX coordinates.
            Assert.Equal(expected, fawn.X, 5);
        }

        [Fact]
        public void MovingTailToRespawnPositionPreservesWebVelocities()
        {
            World world = new(Vector2.Zero);
            Body body = world.CreateCircle(0.4f, Vector2.Zero, density: 1f, dynamic: true);
            BaloonTail tail = new(world, body);
            tail.End.LinearVelocity = new Vector2(2f, 3f);
            tail.SetPositions(Vector2.One);
            Assert.Equal(new Vector2(2f, 3f), tail.End.LinearVelocity);
        }

        [Fact]
        public void StartingDragPreservesTheSpringingDynamicEndpoint()
        {
            World world = new(Vector2.Zero);
            Body body = world.CreateCircle(0.4f, Vector2.Zero, density: 1f, dynamic: true);
            BaloonTail tail = new(world, body);
            tail.SpringTail(0f);
            tail.SetDragging(true, 0.1f);
            Assert.True(tail.Springing);
            Assert.Equal(BodyType.Dynamic, tail.End.BodyType);
        }

        [Fact]
        public void TailAttachmentMakesEndpointsDynamicAndReleaseRestoresKinematicEndpoints()
        {
            World world = new(Vector2.Zero);
            Body body = world.CreateCircle(0.4f, Vector2.Zero, density: 1f, dynamic: true);
            BaloonTail tail = new(world, body);
            world.ProcessChanges();
            Assert.True(tail.End.Position.Y < body.Position.Y);
            tail.SetLinked(true);
            world.ProcessChanges();
            Assert.Equal(BodyType.Dynamic, tail.Start.BodyType);
            Assert.Equal(BodyType.Dynamic, tail.End.BodyType);
            Assert.Equal(3, world.JointList.Count);
            tail.SetLinked(false);
            world.ProcessChanges();
            Assert.Equal(BodyType.Kinematic, tail.Start.BodyType);
            Assert.Equal(BodyType.Kinematic, tail.End.BodyType);
            Assert.Equal(2, world.JointList.Count);
        }

        [Fact]
        public void CompanionImplementsInteractionsWithoutReplacingPrimaryHero()
        {
            Assert.True(typeof(FurBodyClip).IsAssignableFrom(typeof(BaloonBodyClip)));
            Assert.False(typeof(HeroBodyClip).IsAssignableFrom(typeof(BaloonBodyClip)));
            Assert.True(typeof(ISnotLinked).IsAssignableFrom(typeof(BaloonBodyClip)));
            Assert.True(typeof(ITeleportable).IsAssignableFrom(typeof(BaloonBodyClip)));
            Assert.True(typeof(IClickable).IsAssignableFrom(typeof(BaloonBodyClip)));
        }

        [Fact]
        public void LiftIncreasesWithSnotLoadAndWeakensNearCeiling()
        {
            Assert.Equal(28f, BaloonBodyClip.LiftAcceleration(0, 5f, 20f, false));
            Assert.Equal(34f, BaloonBodyClip.LiftAcceleration(3, 5f, 20f, false));
            Assert.Equal(22f / 3f, BaloonBodyClip.LiftAcceleration(0, 20f, 20f, false));
            Assert.True(BaloonBodyClip.LiftAcceleration(0, 16f, 20f, true) < BaloonBodyClip.LiftAcceleration(0, 16f, 20f, false));
        }

        [Fact]
        public void DragMovesTailTowardTargetAndSpringSettlesNearCompanion()
        {
            World world = new(Vector2.Zero);
            Body body = world.CreateCircle(0.4f, Vector2.Zero, density: 1f, dynamic: true);
            BaloonTail tail = new(world, body);
            tail.SetDragging(true, 0f);
            tail.TargetPosition = new Vector2(2f, 0f);
            tail.Update(1f / 60f, 0.25f);
            Assert.True(tail.End.LinearVelocity.X > 0f);
            tail.SetDragging(false, 0.25f);
            tail.SetPositions(body.Position);
            tail.Update(1f / 60f, 0.3f);
            Assert.False(tail.Springing);
            Assert.Equal(BodyType.Kinematic, tail.End.BodyType);
        }
    }
}
