namespace Default.Namespace;

public class LinkedListItem
{
    protected LinkedListItem next;

    protected LinkedListItem previous;

    protected object item;

    public LinkedListItem Previous
    {
        get
        {
            return previous;
        }
        set
        {
            previous = value;
        }
    }

    public LinkedListItem Next
    {
        get
        {
            return next;
        }
        set
        {
            next = value;
        }
    }

    public object Item
    {
        get
        {
            return item;
        }
        set
        {
            item = value;
        }
    }

    public LinkedListItem(object _item)
    {
        item = _item;
    }

    public void Remove()
    {
        if (next != null)
        {
            next.Previous = previous;
        }
        if (previous != null)
        {
            previous.Next = next;
        }
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
