using Mokus2D.Util;

namespace Default.Namespace;

public class PlasticineProcessor : TypeProcessorBase
{
    public PlasticineProcessor(LevelBuilderBase _builder)
        : base("plasticine", _builder)
    {
    }

    public override object ProcessItem(Hashtable item)
    {
        return item.GetArrayList("points").ToVectorList();
    }
}
