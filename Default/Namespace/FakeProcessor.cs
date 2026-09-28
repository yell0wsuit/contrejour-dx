namespace Default.Namespace;

public class FakeProcessor(string _type, LevelBuilderBase _builder) : TypeProcessorBase(_type, _builder)
{
    private static readonly int STATIC_RESULT = 1;

    public override object ProcessItem(Hashtable item)
    {
        return STATIC_RESULT;
    }
}
