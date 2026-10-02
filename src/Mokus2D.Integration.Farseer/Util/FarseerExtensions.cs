using System.Collections.Generic;
using System.Numerics;

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;

using Mokus2D.Integration.Farseer.Physics;

namespace Mokus2D.Integration.Farseer.Util
{
    public static class FarseerExtensions
    {
        public static void SetVelocityX(this Body body, float value, bool resetDynamics = false)
        {
            Vector2 linearVelocity = new(value, body.LinearVelocity.Y);
            if (resetDynamics)
            {
                body.ResetDynamics();
            }
            body.LinearVelocity = linearVelocity;
        }

        public static void SetVelocityY(this Body body, float value, bool resetDynamics = false)
        {
            Vector2 linearVelocity = new(body.LinearVelocity.X, value);
            if (resetDynamics)
            {
                body.ResetDynamics();
            }
            body.LinearVelocity = linearVelocity;
        }

        public static void SetX(this Body body, float value)
        {
            body.Position = new Vector2(value, body.Position.Y);
        }

        public static void SetY(this Body body, float value)
        {
            body.Position = new Vector2(body.Position.X, value);
        }

        public static IDictionary<string, string> GetConfig(this Body body)
        {
            return body.UserData is BodyClip bodyClip ? bodyClip.Config : body.UserData as IDictionary<string, string>;
        }

        public static void SetAsBox(this PolygonShape shape, float halfWidth, float halfHeight)
        {
            shape.Vertices = PolygonTools.CreateRectangle(halfWidth, halfHeight);
        }

        public static void SetAsBox(this PolygonShape shape, float halfWidth, float halfHeight, Vector2 center, float angle)
        {
            shape.Vertices = PolygonTools.CreateRectangle(halfWidth, halfHeight, center, angle);
        }
    }
}
