using Mokus2D.Interfaces;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class AlphaForeground : ForegroundBase, IUpdatable
    {
        private readonly CosChanger changer;

        public AlphaForeground(ContreJourLevelBuilder builder, object body, Node clip, Hashtable config)
            : base(builder, body, clip, config)
        {
            float num = config.GetFloat("alphaStep");
            changer = new CosChanger(num, num)
            {
                MaxValue = config.GetFloat("maximumAlpha"),
                MinValue = config.GetFloat("minimumAlpha")
            };
        }

        public override void Update(float time)
        {
            changer.Update(time);
            Clip.OpacityFloat = changer.Value;
        }
    }
}
