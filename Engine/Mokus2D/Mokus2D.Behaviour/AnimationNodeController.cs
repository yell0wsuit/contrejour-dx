using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace Mokus2D.Behaviour;

public abstract class AnimationNodeController(AnimationNode node) : INodeController, IUpdatable
{
    protected readonly AnimationNode Node = node;

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
