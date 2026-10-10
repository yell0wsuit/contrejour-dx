using Mokus2D.Graphics;
using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class StoneBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config) : ContreJourDXBodyClip(builder, body, clip, config)
    {
        public override void Update(float time)
        {
            base.Update(time);
            Clip.Color = Color.White * Game.LightPower;
        }
    }
}
