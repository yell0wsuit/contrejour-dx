using Mokus2D.Util;

namespace Default.Namespace;

public class PlasticineProcessor(LevelBuilderBase _builder) : TypeProcessorBase("plasticine", _builder)
{
    public override object ProcessItem(Hashtable item)
    {
        return item.GetArrayList("points").ToVectorList();
    }
}
