using System;
using System.Numerics;

using FarseerPhysics.Dynamics;

namespace FarseerPhysics.Controllers
{
    public class SimpleWindForce : AbstractForceController
    {
        public Vector2 Direction { get; set; }

        public float Divergence { get; set; }

        public bool IgnorePosition { get; set; }

        public override void ApplyForce(float dt, float strength)
        {
            foreach (Body body in World.BodyList)
            {
                float decayMultiplier = GetDecayMultiplier(body);
                if (decayMultiplier == 0f)
                {
                    continue;
                }
                Vector2 vector;
                if (ForceType == ForceTypes.Point)
                {
                    vector = body.Position - Position;
                }
                else
                {
                    Direction = Vector2.Normalize(Direction);
                    vector = Direction;
                    if (vector.Length() == 0f)
                    {
                        vector = new Vector2(0f, 1f);
                    }
                }
                if (Variation != 0f)
                {
                    float num = (float)Randomize.NextDouble() * Math.Clamp(Variation, 0f, 1f);
                    vector = Vector2.Normalize(vector);
                    body.ApplyForce(vector * strength * decayMultiplier * num);
                }
                else
                {
                    vector = Vector2.Normalize(vector);
                    body.ApplyForce(vector * strength * decayMultiplier);
                }
            }
        }
    }
}
