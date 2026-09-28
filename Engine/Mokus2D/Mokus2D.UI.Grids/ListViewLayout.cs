using System.Collections.Generic;

using Mokus2D.UI.Layout;
using Mokus2D.Visual;

namespace Mokus2D.UI.Grids;

public class ListViewLayout<T>(ListView<T> listView, Node container) : VerticalLayout(container)
{
    private readonly ListView<T> _listView = listView;

    protected override IList<Node> LayoutNodes => _listView.ItemRenderers;
}
