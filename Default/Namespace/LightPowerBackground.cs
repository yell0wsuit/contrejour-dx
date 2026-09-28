using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Default.Namespace;

public class LightPowerBackground : BackgroundBase
{
    public LightPowerBackground(Node _node, Hashtable _config, ContreJourGame _game)
        : base(_node, _config, _game)
    {
    }

    public override void Update(float time)
    {
        base.Update(time);
        node.Color = Color.White * ((game.LightPower + 0.3f) / 1.3f);
    }
}
