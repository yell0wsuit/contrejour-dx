namespace Default.Namespace;

public class LinkedListItem(object item)
{
    public object Item { get; set; } = item;

    public LinkedListItem Previous { get; set; }

    public LinkedListItem Next { get; set; }

    public void Remove()
    {
        Next?.Previous = Previous;
        Previous?.Next = Next;
        Next = null;
        Previous = null;
    }

    public void InsertBefore(LinkedListItem value)
    {
        if (Previous != null)
        {
            Previous.Next = value;
            value.Previous = Previous;
        }
        value.Next = this;
        Previous = value;
    }

    public void InsertAfter(LinkedListItem value)
    {
        if (Next != null)
        {
            Next.Previous = value;
            value.Next = Next;
        }
        value.Previous = this;
        Next = value;
    }
}
