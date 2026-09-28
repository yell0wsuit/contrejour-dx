using System.Collections.Generic;
using Mokus2D.Visual;

namespace Mokus2D.Parallax;

public class VisibleIslandChildren : LinkedList<Node>
{
	public readonly VisibleIsland Island;

	public VisibleIslandChildren(VisibleIsland island)
	{
		Island = island;
	}
}
