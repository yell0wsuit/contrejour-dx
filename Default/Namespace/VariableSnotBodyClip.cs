using Mokus2D.Visual;

namespace Default.Namespace;

public class VariableSnotBodyClip : SnotBodyClip
{
    private VariableSnotBodyClip(LevelBuilderBase _builder, SnotData _body, Node _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
        _ = new SnotVariator(this);
    }
}
