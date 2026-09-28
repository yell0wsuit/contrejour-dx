namespace Default.Namespace;

public class LinkedListItem(object _item)
{
    private LinkedListItem next;

    private LinkedListItem previous;

    protected object item = _item;

    public LinkedListItem Previous
    {
        get => previous;
        set => previous = value;
    }

    public LinkedListItem Next
    {
        get => next;
        set => next = value;
    }

    public object Item
    {
        get => item;
        set => item = value;
    }

    public void Remove()
    {
        next?.Previous = previous;
        previous?.Next = next;
        Next = null;
        Previous = null;
    }

    public void InsertBefore(LinkedListItem value)
    {
        if (previous != null)
        {
            previous.Next = value;
            value.Previous = previous;
        }
        value.Next = this;
        Previous = value;
    }

    public void InsertAfter(LinkedListItem value)
    {
        if (next != null)
        {
            next.Previous = value;
            value.Next = next;
        }
        value.Previous = this;
        Next = value;
    }
}
