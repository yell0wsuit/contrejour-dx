using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class LightPointProcessor : TypeProcessorBase
{
    public LightPointProcessor(LevelBuilderBase _builder)
        : base("lightPoint", _builder)
    {
    }

    public override object ProcessItem(Hashtable item)
    {
        Vector2 vector = item.GetVector("position");
        ((ContreJourGame)builder.Game).LightPoint = vector;
        return null;
    }
}
