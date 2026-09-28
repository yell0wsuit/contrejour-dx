namespace Default.Namespace;

public class FakeProcessor : TypeProcessorBase
{
    private static readonly int STATIC_RESULT = 1;

    public FakeProcessor(string _type, LevelBuilderBase _builder)
        : base(_type, _builder)
    {
    }

    public override object ProcessItem(Hashtable item)
    {
        return STATIC_RESULT;
    }
}
