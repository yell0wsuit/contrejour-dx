using Microsoft.Xna.Framework;

namespace ContreJour.Gameplay;

public class LightPointProcessor(LevelBuilderBase builder) : TypeProcessorBase("lightPoint", builder)
{
    public override object ProcessItem(Hashtable item)
    {
        Vector2 vector = item.GetVector("position");
        ((ContreJourGame)Builder.Game).LightPoint = vector;
        return null;
    }
}
