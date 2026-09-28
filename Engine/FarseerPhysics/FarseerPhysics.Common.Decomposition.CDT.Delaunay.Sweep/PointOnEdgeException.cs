using System;

namespace FarseerPhysics.Common.Decomposition.CDT.Delaunay.Sweep;

internal sealed class PointOnEdgeException : NotImplementedException
{
    public PointOnEdgeException(string message)
        : base(message)
    {
    }
}
