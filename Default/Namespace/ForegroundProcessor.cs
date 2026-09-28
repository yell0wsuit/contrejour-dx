namespace Default.Namespace;

public class ForegroundProcessor : TypeProcessorBase
{
    private static int STATIC_RESULT = 1;

    public ForegroundProcessor(LevelBuilderBase _builder)
        : base("foreground", _builder)
    {
    }

    public override object ProcessItem(Hashtable item)
    {
        Hashtable hashtable = item.GetHashtable("config");
        if (hashtable.NotExists("z"))
        {
            hashtable["z"] = "12";
        }
        if (hashtable.NotExists("clipType"))
        {
            hashtable["skipClip"] = "true";
        }
        return STATIC_RESULT;
    }
}
