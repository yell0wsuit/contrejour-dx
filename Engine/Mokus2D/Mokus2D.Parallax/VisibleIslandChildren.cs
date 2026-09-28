using System.Collections.Generic;

using Mokus2D.Visual;

namespace Mokus2D.Parallax;

public class VisibleIslandChildren(VisibleIsland island) : LinkedList<Node>
{
    public readonly VisibleIsland Island = island;
}
