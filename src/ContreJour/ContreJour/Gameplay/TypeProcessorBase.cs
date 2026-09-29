namespace ContreJour.Gameplay;

public class TypeProcessorBase(string type, LevelBuilderBase builder)
{
    private readonly string type = type;

    protected LevelBuilderBase Builder { get; set; } = builder;

    public virtual bool Match(Hashtable item)
    {
        return item.GetString("config/type") == type;
    }

    public virtual object ProcessItem(Hashtable item)
    {
        return null;
    }
}
