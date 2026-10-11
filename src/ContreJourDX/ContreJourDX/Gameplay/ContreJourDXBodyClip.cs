using System.Numerics;

using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class ContreJourDXBodyClip : BodyClip
    {
        private readonly ContreJourDXLevelBuilder contreJourBuilder;

        public ContreJourDXGame Game { get; }

        public ContreJourDXBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
            : base(builder, body, clip, config)
        {
            contreJourBuilder = (ContreJourDXLevelBuilder)builder;
            Game = contreJourBuilder.ContreJourDX;
        }

        public virtual float TouchDistance(Vector2 touchPosition)
        {
            return Vector2.Distance(PositionVec, touchPosition);
        }
    }
}
