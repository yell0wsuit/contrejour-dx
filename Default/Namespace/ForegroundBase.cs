using System.Diagnostics.CodeAnalysis;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace Default.Namespace;

public class ForegroundBase : BodyClip, IUpdatable
{
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public ForegroundBase(ContreJourLevelBuilder _builder, object _body, Node _clip, Hashtable _config)
        : base(_builder, null, _clip, _config)
    {
        _builder.ContreJour.AddForeground(this);
    }

    public override void Update(float time)
    {
    }
}
