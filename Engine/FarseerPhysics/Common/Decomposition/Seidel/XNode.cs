namespace FarseerPhysics.Common.Decomposition.Seidel;

internal sealed class XNode(Point point, Node lChild, Node rChild) : Node(lChild, rChild)
{
    private readonly Point _point = point;

    public override Sink Locate(Edge edge)
    {
        return edge.P.X >= _point.X ? RightChild.Locate(edge) : LeftChild.Locate(edge);
    }
}
