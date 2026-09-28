namespace FarseerPhysics.Common.Decomposition.Seidel;

internal class XNode : Node
{
    private Point _point;

    public XNode(Point point, Node lChild, Node rChild)
        : base(lChild, rChild)
    {
        _point = point;
    }

    public override Sink Locate(Edge edge)
    {
        return edge.P.X >= _point.X ? RightChild.Locate(edge) : LeftChild.Locate(edge);
    }
}
