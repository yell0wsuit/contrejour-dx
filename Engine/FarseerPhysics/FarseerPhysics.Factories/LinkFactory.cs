using System.Collections.Generic;

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Factories;

public static class LinkFactory
{
    public static Path CreateChain(World world, Vector2 start, Vector2 end, float linkWidth, float linkHeight, int numberOfLinks, float linkDensity, bool attachRopeJoint)
    {
        Path path = new();
        path.Add(start);
        path.Add(end);
        PolygonShape shape = new(PolygonTools.CreateRectangle(linkWidth, linkHeight), linkDensity);
        List<Body> list = PathManager.EvenlyDistributeShapesAlongPath(world, path, shape, BodyType.Dynamic, numberOfLinks);
        _ = PathManager.AttachBodiesWithRevoluteJoint(world, list, new Vector2(0f, 0f - linkHeight), new Vector2(0f, linkHeight), connectFirstAndLast: false, collideConnected: false);
        if (attachRopeJoint)
        {
            _ = JointFactory.CreateRopeJoint(world, list[0], list[^1], Vector2.Zero, Vector2.Zero);
        }
        return path;
    }
}
