using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Default.Namespace;

public class LightPowerBackground(Node _node, Hashtable config, ContreJourGame _game) : BackgroundBase(_node, config, _game)
{
    public override void Update(float time)
    {
        base.Update(time);
        node.Color = Color.White * ((game.LightPower + 0.3f) / 1.3f);
    }
}
