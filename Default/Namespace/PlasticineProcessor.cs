using Mokus2D.Util;

namespace Default.Namespace;

public class PlasticineProcessor(LevelBuilderBase builder) : TypeProcessorBase("plasticine", builder)
{
    public override object ProcessItem(Hashtable item)
    {
        return item.GetArrayList("points").ToVectorList();
    }
}
