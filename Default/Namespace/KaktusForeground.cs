using Mokus2D.Visual;

namespace Default.Namespace;

public class KaktusForeground : ForegroundBase
{
    public KaktusForeground(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        string text = builder.ContreJour.ChooseSide("Black", null, "_5", null, "_6");
        if (text != null)
        {
            clip = (Sprite)LevelBuilderBase.ReplaceClipWith(clip, Config.GetString("viewType") + text);
            builder.ChangeChildLayer(clip, -2);
        }
    }
}
