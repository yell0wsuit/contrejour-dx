using System.Diagnostics.CodeAnalysis;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Gameplay;

public class ForegroundBase : BodyClip, IUpdatable
{
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public ForegroundBase(ContreJourLevelBuilder builder, object body, Node clip, Hashtable config)
        : base(builder, null, clip, config)
    {
        builder.ContreJour.AddForeground(this);
    }

    public override void Update(float time)
    {
    }
}
