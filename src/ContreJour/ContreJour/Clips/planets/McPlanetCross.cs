using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McPlanetCross : Sprite, IFreeable, IId
    {
        public const string ID = "planets/McPlanetCross";

        public string Id => "planets/McPlanetCross";

        public static McPlanetCross New()
        {
            McPlanetCross mcPlanetCross = StaticPool.New<McPlanetCross>();
            mcPlanetCross.RefreshProperties();
            return mcPlanetCross;
        }

        public McPlanetCross()
            : base("planets/McPlanetCross")
        {
        }

        public void Free()
        {
            StaticPool.Free<McPlanetCross>(this);
        }
    }
}
