using System.Collections.Generic;

using FarseerPhysics.Common.Decomposition.CDT.Delaunay.Sweep;

namespace FarseerPhysics.Common.Decomposition.CDT
{
    internal class TriangulationPoint(double x, double y)
    {
        public double X = x;

        public double Y = y;

        public List<DTSweepConstraint> Edges { get; private set; }

        public float Xf
        {
            get => (float)X;
            set => X = value;
        }

        public float Yf
        {
            get => (float)Y;
            set => Y = value;
        }

        public bool HasEdges => Edges != null;

        public override string ToString()
        {
            return "[" + X + "," + Y + "]";
        }

        public void AddEdge(DTSweepConstraint e)
        {
            Edges ??= [];
            Edges.Add(e);
        }
    }
}
