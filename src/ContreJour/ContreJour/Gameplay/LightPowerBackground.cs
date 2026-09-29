using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace ContreJour.Gameplay;

public class LightPowerBackground(Node node, Hashtable config, ContreJourGame game) : BackgroundBase(node, config, game)
{
    public override void Update(float time)
    {
        base.Update(time);
        Node.Color = Color.White * ((Game.LightPower + 0.3f) / 1.3f);
    }
}
