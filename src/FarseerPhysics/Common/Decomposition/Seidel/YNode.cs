namespace FarseerPhysics.Common.Decomposition.Seidel
{
    internal sealed class YNode(Edge edge, Node lChild, Node rChild) : Node(lChild, rChild)
    {
        private readonly Edge _edge = edge;

        public override Sink Locate(Edge edge)
        {
            return _edge.IsAbove(edge.P)
                ? RightChild.Locate(edge)
                : _edge.IsBelow(edge.P) ? LeftChild.Locate(edge) : edge.Slope < _edge.Slope ? RightChild.Locate(edge) : LeftChild.Locate(edge);
        }
    }
}
