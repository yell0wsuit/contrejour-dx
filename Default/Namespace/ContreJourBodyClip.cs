using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Default.Namespace;

public class ContreJourBodyClip : BodyClip
{
    private readonly ContreJourLevelBuilder contreJourBuilder;

    public ContreJourGame Game { get; }

    public ContreJourBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        contreJourBuilder = (ContreJourLevelBuilder)builder;
        Game = contreJourBuilder.ContreJour;
    }

    public virtual float TouchDistance(Vector2 touchPosition)
    {
        return PositionVec.DistanceTo(touchPosition);
    }
}
