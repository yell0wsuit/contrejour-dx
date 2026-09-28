using System.Collections.Generic;

namespace Mokus2D.Visual.Data;

internal class NodeComparer : IComparer<Node>
{
    public int Compare(Node x, Node y)
    {
        if (x == y)
        {
            return 0;
        }
        return x.Layer > y.Layer ? 1 : -1;
    }
}
