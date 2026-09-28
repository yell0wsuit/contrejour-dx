using Mokus2D.Visual;

namespace Mokus2D.UI.Layout;

public abstract class LayoutBase
{
    protected readonly Node Container;

    protected LayoutBase(Node container)
    {
        Container = container;
    }
}
