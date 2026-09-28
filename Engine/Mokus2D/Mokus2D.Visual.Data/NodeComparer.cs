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
        if (x.Layer > y.Layer)
        {
            return 1;
        }
        return -1;
    }
}
