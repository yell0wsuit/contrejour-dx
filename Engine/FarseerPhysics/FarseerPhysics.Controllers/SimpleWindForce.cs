using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Controllers;

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
				vector = body.Position - base.Position;
			}
			else
			{
				Direction.Normalize();
				vector = Direction;
				if (vector.Length() == 0f)
				{
					vector = new Vector2(0f, 1f);
				}
			}
			if (base.Variation != 0f)
			{
				float num = (float)Randomize.NextDouble() * MathHelper.Clamp(base.Variation, 0f, 1f);
				vector.Normalize();
				body.ApplyForce(vector * strength * decayMultiplier * num);
			}
			else
			{
				vector.Normalize();
				body.ApplyForce(vector * strength * decayMultiplier);
			}
		}
	}
}
