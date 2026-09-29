using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McPlanetRoseLight : Sprite, IFreeable, IId
    {
        public const string ID = "planets/McPlanetRoseLight";

        public string Id => "planets/McPlanetRoseLight";

        public static McPlanetRoseLight New()
        {
            McPlanetRoseLight mcPlanetRoseLight = StaticPool.New<McPlanetRoseLight>();
            mcPlanetRoseLight.RefreshProperties();
            return mcPlanetRoseLight;
        }

        public McPlanetRoseLight()
            : base("planets/McPlanetRoseLight")
        {
        }

        public void Free()
        {
            StaticPool.Free<McPlanetRoseLight>(this);
        }
    }
}
