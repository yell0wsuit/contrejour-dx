namespace FarseerPhysics.Common.Decomposition.Seidel;

internal class YNode : Node
{
    private Edge _edge;

    public YNode(Edge edge, Node lChild, Node rChild)
        : base(lChild, rChild)
    {
        _edge = edge;
    }

    public override Sink Locate(Edge edge)
    {
        if (_edge.IsAbove(edge.P))
        {
            return RightChild.Locate(edge);
        }
        return _edge.IsBelow(edge.P) ? LeftChild.Locate(edge) : edge.Slope < _edge.Slope ? RightChild.Locate(edge) : LeftChild.Locate(edge);
    }
}
