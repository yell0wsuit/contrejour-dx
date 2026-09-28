using System;

namespace Mokus2D.UI.Grids;

public interface IListView
{
    int ItemsCount { get; }

    int DataCount { get; }

    float ItemsPosition { get; set; }

    event Action DataChangedEvent;
}
