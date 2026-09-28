namespace Mokus2D.Visual.Exceptions;

public class ChildNotFoundException : NodeException
{
    public string Id { get; }

    public ChildNotFoundException(string id)
    {
        Id = id;
    }
}
