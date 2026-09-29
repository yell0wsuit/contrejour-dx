using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace ContreJour.Gameplay;

public class StoneBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config) : ContreJourBodyClip(builder, body, clip, config)
{
    public override void Update(float time)
    {
        base.Update(time);
        Clip.Color = Color.White * Game.LightPower;
    }
}
