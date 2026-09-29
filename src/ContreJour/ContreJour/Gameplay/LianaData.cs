using System.Collections.Generic;
using System.Numerics;

using FarseerPhysics.Dynamics;

namespace ContreJour.Gameplay
{
    public class LianaData : ILianaDrawData
    {
        public List<Body> Bodies { get; } = [];

        public void AddBody(Body body)
        {
            Bodies.Add(body);
        }

        public int PointsCount()
        {
            return Bodies.Count;
        }

        public Vector2 PositionAt(int index)
        {
            return Bodies[index].Position;
        }
    }
}
