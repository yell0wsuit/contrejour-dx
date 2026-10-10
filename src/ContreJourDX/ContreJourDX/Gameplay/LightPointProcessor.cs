using System.Numerics;

namespace ContreJourDX.Gameplay
{
    public class LightPointProcessor(LevelBuilderBase builder) : TypeProcessorBase("lightPoint", builder)
    {
        public override object ProcessItem(Hashtable item)
        {
            Vector2 vector = item.GetVector("position");
            ((ContreJourDXGame)Builder.Game).LightPoint = vector;
            return null;
        }
    }
}
