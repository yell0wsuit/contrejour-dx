using System.Numerics;

using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class PlanetSnotContainer : Node, IDepthDependent
    {
        private readonly PlanetSnot _snot;

        private readonly PlanetSnotEye _eye = new(null, visible: true, Vector2.Zero);

        public float Depth
        {
            set => _snot.Depth = value;
        }

        public PlanetSnotContainer()
        {
            _snot = new PlanetSnot(_eye);
            AddChild(_snot);
            AddChild(_snot.BaseSprite);
            AddChild(_eye);
        }
    }
}
