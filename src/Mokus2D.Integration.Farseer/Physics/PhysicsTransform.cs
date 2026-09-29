using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;

namespace Mokus2D.Integration.Farseer.Physics
{
    public class PhysicsTransform(float physicsToPixels)
    {
        public float PhysicsToPixels { get; protected set; } = physicsToPixels;

        public Vector2 ToPhysics(float x, float y)
        {
            return ToPhysics(new Vector2(x, y));
        }

        public float ToPhysics(float pixels)
        {
            return pixels / PhysicsToPixels;
        }

        public Vector2 ToPhysics(Vector2 pixels)
        {
            return XnaMath.Divide(pixels, PhysicsToPixels);
        }

        public float ToPixels(float physicsPosition)
        {
            return physicsPosition * PhysicsToPixels;
        }

        public Vector2 ToPixels(Vector2 physicsPosition)
        {
            return physicsPosition * PhysicsToPixels;
        }
    }
}
