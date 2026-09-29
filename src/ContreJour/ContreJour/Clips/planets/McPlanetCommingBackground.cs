using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McPlanetCommingBackground : Sprite, IFreeable, IId
    {
        public const string ID = "planets/McPlanetCommingBackground";

        public string Id => "planets/McPlanetCommingBackground";

        public static McPlanetCommingBackground New()
        {
            McPlanetCommingBackground mcPlanetCommingBackground = StaticPool.New<McPlanetCommingBackground>();
            mcPlanetCommingBackground.RefreshProperties();
            return mcPlanetCommingBackground;
        }

        public McPlanetCommingBackground()
            : base("planets/McPlanetCommingBackground")
        {
        }

        public void Free()
        {
            StaticPool.Free<McPlanetCommingBackground>(this);
        }
    }
}
