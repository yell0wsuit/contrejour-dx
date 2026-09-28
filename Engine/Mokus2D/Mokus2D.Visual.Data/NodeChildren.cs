using System.Collections.Generic;

namespace Mokus2D.Visual.Data;

public class NodeChildren : SortedList<Node>
{
	private static readonly NodeComparer Comparer = new NodeComparer();

	public NodeChildren()
		: base((IComparer<Node>)Comparer, 64)
	{
	}

	public override bool Remove(Node item)
	{
		return Items.Remove(item);
	}

	public override void Insert(int index, Node item)
	{
		Items.Insert(index, item);
	}
}
