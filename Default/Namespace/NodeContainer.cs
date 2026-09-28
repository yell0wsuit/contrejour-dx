using System;

using Mokus2D.Visual;

namespace Default.Namespace;

public class NodeContainer(Func<Node> nodeFactory) : Node
{
    private readonly Func<Node> _nodeFactory = nodeFactory;

    private Node _node;

    protected override void OnAddedToStage()
    {
        base.OnAddedToStage();
        _node = _nodeFactory();
        AddChild(_node);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _node?.Dispose();
    }
}
