namespace Default.Namespace;

public class BackSnotProcessor : SnotProcessor
{
    public BackSnotProcessor(LevelBuilderBase _builder)
        : base(_builder, "backSnot", 100f * _builder.EngineConfig.SizeMultiplier)
    {
    }

    public override float GetDensityTotal(int index, int total)
    {
        return 0.3f;
    }
}
