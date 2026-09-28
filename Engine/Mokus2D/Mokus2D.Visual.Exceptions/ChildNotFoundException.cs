namespace Mokus2D.Visual.Exceptions;

public class ChildNotFoundException : NodeException
{
    private readonly string id;

    public string Id => id;

    public ChildNotFoundException(string id)
    {
        this.id = id;
    }
}
