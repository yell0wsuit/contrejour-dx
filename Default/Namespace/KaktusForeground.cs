using Mokus2D.Visual;

namespace Default.Namespace;

public class KaktusForeground : ForegroundBase
{
    public KaktusForeground(ContreJourLevelBuilder _builder, object _body, Sprite _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
        string text = _builder.ContreJour.ChooseSide("Black", null, "_5", null, "_6");
        if (text != null)
        {
            _clip = (Sprite)LevelBuilderBase.ReplaceClipWith(_clip, config.GetString("viewType") + text);
            _builder.ChangeChildLayer(_clip, -2);
        }
    }
}
