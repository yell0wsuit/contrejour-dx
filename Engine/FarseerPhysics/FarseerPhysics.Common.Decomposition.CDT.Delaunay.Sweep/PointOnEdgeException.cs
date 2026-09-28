using System;

namespace FarseerPhysics.Common.Decomposition.CDT.Delaunay.Sweep;

internal sealed class PointOnEdgeException(string message) : NotImplementedException(message)
{
}
