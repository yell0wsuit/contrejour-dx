using Mokus2D.Visual;

namespace Default.Namespace;

public class KaktusBodyClip : ContreJourBodyClip
{
    public KaktusBodyClip(ContreJourLevelBuilder _builder, object _body, Sprite _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
        string text = _builder.ContreJour.ChooseSide("Black", null, "_5", null, "_6");
        if (text != null)
        {
            _clip = (Sprite)_builder.ReplaceClipWith(_clip, config.GetString("viewType") + text);
        }
        _clip.Parent.ChangeChildLayer(_clip, -2);
    }
}
