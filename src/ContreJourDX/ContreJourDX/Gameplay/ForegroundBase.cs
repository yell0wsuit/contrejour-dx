using System.Diagnostics.CodeAnalysis;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class ForegroundBase : BodyClip, IUpdatable
    {
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
        public ForegroundBase(ContreJourDXLevelBuilder builder, object body, Node clip, Hashtable config)
            : base(builder, null, clip, config)
        {
            builder.ContreJourDX.AddForeground(this);
        }

        public override void Update(float time)
        {
        }
    }
}
