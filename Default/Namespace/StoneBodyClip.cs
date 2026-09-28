using Microsoft.Xna.Framework;
using Mokus2D.Visual;

namespace Default.Namespace;

public class StoneBodyClip : ContreJourBodyClip
{
    public StoneBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
        : base(builder, body, clip, config)
    {
    }

    public override void Update(float time)
    {
        base.Update(time);
        clip.Color = Color.White * base.Game.LightPower;
    }
}
