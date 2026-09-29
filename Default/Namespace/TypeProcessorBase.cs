namespace Default.Namespace;

public class TypeProcessorBase(string _type, LevelBuilderBase _builder)
{
    private readonly string type = _type;

    protected LevelBuilderBase Builder { get; set; } = _builder;

    public virtual bool Match(Hashtable item)
    {
        return item.GetString("config/type") == type;
    }

    public virtual object ProcessItem(Hashtable item)
    {
        return null;
    }
}
