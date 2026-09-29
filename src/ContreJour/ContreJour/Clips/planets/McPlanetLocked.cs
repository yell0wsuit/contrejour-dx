using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McPlanetLocked : Sprite, IFreeable, IId
    {
        public const string ID = "planets/McPlanetLocked";

        public string Id => "planets/McPlanetLocked";

        public static McPlanetLocked New()
        {
            McPlanetLocked mcPlanetLocked = StaticPool.New<McPlanetLocked>();
            mcPlanetLocked.RefreshProperties();
            return mcPlanetLocked;
        }

        public McPlanetLocked()
            : base("planets/McPlanetLocked")
        {
        }

        public void Free()
        {
            StaticPool.Free<McPlanetLocked>(this);
        }
    }
}
