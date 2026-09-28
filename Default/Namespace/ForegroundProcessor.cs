namespace Default.Namespace;

public class ForegroundProcessor(LevelBuilderBase _builder) : TypeProcessorBase("foreground", _builder)
{
    private static readonly int STATIC_RESULT = 1;

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
