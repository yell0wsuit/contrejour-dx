using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace Mokus2D.Behaviour;

public abstract class NodeController<T> : INodeController, IUpdatable where T : Node
{
    protected T Node { get; }

    protected NodeController(T node)
    {
        Node = node;
        node.Controller = this;
    }

    public virtual void Update(float time)
    {
    }

    public virtual void OnRemovedFromStage()
    {
    }

    public virtual void OnAddedToStage()
    {
    }

    public virtual void FirstUpdate()
    {
    }
}
