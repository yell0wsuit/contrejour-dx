using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class LightPointProcessor(LevelBuilderBase builder) : TypeProcessorBase("lightPoint", builder)
{
    public override object ProcessItem(Hashtable item)
    {
        Vector2 vector = item.GetVector("position");
        ((ContreJourGame)builder.Game).LightPoint = vector;
        return null;
    }
}
