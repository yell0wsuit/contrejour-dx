using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Interactive
{
    public interface IMouseOverNode : IBoundsNode, ISizeNode
    {
        void MouseOver();

        void MouseOut();
    }
}
