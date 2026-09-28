namespace Default.Namespace;

public class TypeProcessorBase
{
    protected string type;

    protected LevelBuilderBase builder;

    public TypeProcessorBase(string _type, LevelBuilderBase _builder)
    {
        type = _type;
        builder = _builder;
    }

    public virtual bool Match(Hashtable item)
    {
        return item.GetString("config/type") == type;
    }

    public virtual object ProcessItem(Hashtable item)
    {
        return null;
    }
}
