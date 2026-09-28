namespace Mokus2D.Visual.Data;

public class NodeChildren : SortedCollection<Node>
{
    private static readonly NodeComparer Comparer = new();

    public NodeChildren()
        : base(Comparer, 64)
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
