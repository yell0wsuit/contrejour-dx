using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Interactive
{
    public interface IClickableNode : IBoundsNode, ISizeNode, ITouchNode
    {
        int ClickablePriority { get; }
    }
}
