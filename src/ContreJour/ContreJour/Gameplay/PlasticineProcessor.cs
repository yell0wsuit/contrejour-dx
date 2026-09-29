using Mokus2D.Util;

namespace ContreJour.Gameplay
{
    public class PlasticineProcessor(LevelBuilderBase builder) : TypeProcessorBase("plasticine", builder)
    {
        public override object ProcessItem(Hashtable item)
        {
            return item.GetArrayList("points").ToVectorList();
        }
    }
}
