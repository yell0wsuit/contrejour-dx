using System.Collections.Generic;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class LianaData : ILianaDrawData
{
    protected List<Body> bodies = new List<Body>();

    public List<Body> Bodies => bodies;

    public void AddBody(Body body)
    {
        bodies.Add(body);
    }

    public int PointsCount()
    {
        return bodies.Count;
    }

    public Vector2 PositionAt(int index)
    {
        return bodies[index].Position;
    }
}
