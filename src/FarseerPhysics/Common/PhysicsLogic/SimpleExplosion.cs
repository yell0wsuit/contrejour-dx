using System;
using System.Collections.Generic;
using System.Numerics;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;

namespace FarseerPhysics.Common.PhysicsLogic
{
    public sealed class SimpleExplosion(World world) : PhysicsLogic(world, PhysicsLogicType.Explosion)
    {
        public float Power { get; set; } = 1f;

        public Dictionary<Body, Vector2> Activate(Vector2 pos, float radius, float force, float maxForce = float.MaxValue)
        {
            HashSet<Body> affectedBodies = [];
            AABB aabb = default;
            aabb.LowerBound = pos - new Vector2(radius);
            aabb.UpperBound = pos + new Vector2(radius);
            World.QueryAABB(delegate (Fixture fixture)
            {
                if (Vector2.Distance(fixture.Body.Position, pos) <= radius && !affectedBodies.Contains(fixture.Body))
                {
                    _ = affectedBodies.Add(fixture.Body);
                }
                return true;
            }, ref aabb);
            return ApplyImpulse(pos, radius, force, maxForce, affectedBodies);
        }

        private Dictionary<Body, Vector2> ApplyImpulse(Vector2 pos, float radius, float force, float maxForce, HashSet<Body> overlappingBodies)
        {
            Dictionary<Body, Vector2> dictionary = new(overlappingBodies.Count);
            foreach (Body overlappingBody in overlappingBodies)
            {
                if (IsActiveOn(overlappingBody))
                {
                    float distance = Vector2.Distance(pos, overlappingBody.Position);
                    float percent = GetPercent(distance, radius);
                    Vector2 vector = pos - overlappingBody.Position;
                    vector *= 1f / (float)Math.Sqrt((vector.X * vector.X) + (vector.Y * vector.Y));
                    vector *= MathF.Min(force * percent, maxForce);
                    vector *= -1f;
                    overlappingBody.ApplyLinearImpulse(vector);
                    dictionary.Add(overlappingBody, vector);
                }
            }
            return dictionary;
        }

        private float GetPercent(float distance, float radius)
        {
            float num = (float)Math.Pow(1f - ((distance - radius) / radius), Power) - 1f;
            return float.IsNaN(num) ? 0f : Math.Clamp(num, 0f, 1f);
        }
    }
}
