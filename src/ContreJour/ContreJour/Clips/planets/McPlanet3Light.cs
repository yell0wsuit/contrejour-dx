using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McPlanet3Light : Sprite, IFreeable, IId
    {
        public const string ID = "planets/McPlanet3Light";

        public string Id => "planets/McPlanet3Light";

        public static McPlanet3Light New()
        {
            McPlanet3Light mcPlanet3Light = StaticPool.New<McPlanet3Light>();
            mcPlanet3Light.RefreshProperties();
            return mcPlanet3Light;
        }

        public McPlanet3Light()
            : base("planets/McPlanet3Light")
        {
        }

        public void Free()
        {
            StaticPool.Free<McPlanet3Light>(this);
        }
    }
}
