using Mokus2D.Visual;

namespace Mokus2D.UI.Layout;

public abstract class LayoutBase(Node container)
{
    protected Node Container { get; } = container;
}
