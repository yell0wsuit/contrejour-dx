namespace Default.Namespace;

public class FakeProcessor(string type, LevelBuilderBase builder) : TypeProcessorBase(type, builder)
{
    private static readonly int STATIC_RESULT = 1;

    public override object ProcessItem(Hashtable item)
    {
        return STATIC_RESULT;
    }
}
