using System.Collections.Generic;

using Mokus2D.UI.Layout;
using Mokus2D.Visual;

namespace Mokus2D.UI.Grids;

public class ListViewLayout<T> : VerticalLayout
{
    private readonly ListView<T> _listView;

    protected override IList<Node> LayoutNodes => _listView.ItemRenderers;

    public ListViewLayout(ListView<T> listView, Node container)
        : base(container)
    {
        _listView = listView;
    }
}
