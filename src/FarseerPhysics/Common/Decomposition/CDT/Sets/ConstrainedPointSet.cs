using System.Collections.Generic;

namespace FarseerPhysics.Common.Decomposition.CDT.Sets
{
    internal sealed class ConstrainedPointSet : PointSet
    {
        private readonly List<TriangulationPoint> _constrainedPointList;

        public int[] EdgeIndex { get; private set; }

        public override TriangulationMode TriangulationMode => TriangulationMode.Constrained;

        public ConstrainedPointSet(List<TriangulationPoint> points, int[] index)
            : base(points)
        {
            EdgeIndex = index;
        }

        public ConstrainedPointSet(List<TriangulationPoint> points, IEnumerable<TriangulationPoint> constraints)
            : base(points)
        {
            _constrainedPointList = [.. constraints];
        }

        public override void PrepareTriangulation(TriangulationContext tcx)
        {
            base.PrepareTriangulation(tcx);
            if (_constrainedPointList != null)
            {
                List<TriangulationPoint>.Enumerator enumerator = _constrainedPointList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TriangulationPoint current = enumerator.Current;
                    _ = enumerator.MoveNext();
                    TriangulationPoint current2 = enumerator.Current;
                    _ = tcx.NewConstraint(current, current2);
                }
            }
            else
            {
                for (int i = 0; i < EdgeIndex.Length; i += 2)
                {
                    _ = tcx.NewConstraint(Points[EdgeIndex[i]], Points[EdgeIndex[i + 1]]);
                }
            }
        }

        public static bool IsValid()
        {
            return true;
        }
    }
}
