using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class KaktusBodyClip : ContreJourDXBodyClip
    {
        public KaktusBodyClip(ContreJourDXLevelBuilder builder, object body, Sprite clip, Hashtable config)
            : base(builder, body, clip, config)
        {
            string text = builder.ContreJourDX.ChooseSide("Black", null, "_5", null, "_6");
            if (text != null)
            {
                clip = (Sprite)LevelBuilderBase.ReplaceClipWith(clip, Config.GetString("viewType") + text);
            }
            clip.Parent.ChangeChildLayer(clip, -2);
        }
    }
}
