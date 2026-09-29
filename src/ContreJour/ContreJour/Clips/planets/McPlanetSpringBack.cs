using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McPlanetSpringBack : Sprite, IFreeable, IId
    {
        public const string ID = "planets/McPlanetSpringBack";

        public string Id => "planets/McPlanetSpringBack";

        public static McPlanetSpringBack New()
        {
            McPlanetSpringBack mcPlanetSpringBack = StaticPool.New<McPlanetSpringBack>();
            mcPlanetSpringBack.RefreshProperties();
            return mcPlanetSpringBack;
        }

        public McPlanetSpringBack()
            : base("planets/McPlanetSpringBack")
        {
        }

        public void Free()
        {
            StaticPool.Free<McPlanetSpringBack>(this);
        }
    }
}
