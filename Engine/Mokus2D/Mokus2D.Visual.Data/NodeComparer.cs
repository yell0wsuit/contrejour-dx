using System.Collections.Generic;

namespace Mokus2D.Visual.Data;

internal sealed class NodeComparer : IComparer<Node>
{
    public int Compare(Node x, Node y)
    {
        return x == y ? 0 : x.Layer > y.Layer ? 1 : -1;
    }
}
