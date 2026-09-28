using System;

using Mokus2D.Visual;

namespace Default.Namespace;

public class NodeContainer : Node
{
    private readonly Func<Node> _nodeFactory;

    private Node _node;

    public NodeContainer(Func<Node> nodeFactory)
    {
        _nodeFactory = nodeFactory;
    }

    protected override void OnAddedToStage()
    {
        base.OnAddedToStage();
        _node = _nodeFactory();
        AddChild(_node);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (_node != null)
        {
            _node.Dispose();
        }
    }
}
