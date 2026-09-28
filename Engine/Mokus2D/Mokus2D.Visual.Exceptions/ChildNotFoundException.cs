namespace Mokus2D.Visual.Exceptions;

public class ChildNotFoundException(string id) : NodeException
{
    public string Id { get; } = id;
}
