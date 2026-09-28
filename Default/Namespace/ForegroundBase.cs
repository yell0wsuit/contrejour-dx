using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace Default.Namespace;

public class ForegroundBase : BodyClip, IUpdatable
{
    public ForegroundBase(ContreJourLevelBuilder _builder, object _body, Node _clip, Hashtable _config)
        : base(_builder, null, _clip, _config)
    {
        _builder.ContreJour.AddForeground(this);
    }

    public override void Update(float time)
    {
    }
}
