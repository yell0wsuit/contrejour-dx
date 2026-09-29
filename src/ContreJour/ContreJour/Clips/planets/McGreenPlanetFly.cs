using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McGreenPlanetFly : Sprite, IFreeable, IId
    {
        public const string ID = "planets/McGreenPlanetFly";

        public string Id => "planets/McGreenPlanetFly";

        public static McGreenPlanetFly New()
        {
            McGreenPlanetFly mcGreenPlanetFly = StaticPool.New<McGreenPlanetFly>();
            mcGreenPlanetFly.RefreshProperties();
            return mcGreenPlanetFly;
        }

        public McGreenPlanetFly()
            : base("planets/McGreenPlanetFly")
        {
        }

        public void Free()
        {
            StaticPool.Free<McGreenPlanetFly>(this);
        }
    }
}
