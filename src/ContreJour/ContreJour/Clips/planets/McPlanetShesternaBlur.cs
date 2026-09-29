using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McPlanetShesternaBlur : Sprite, IFreeable, IId
    {
        public const string ID = "planets/McPlanetShesternaBlur";

        public string Id => "planets/McPlanetShesternaBlur";

        public static McPlanetShesternaBlur New()
        {
            McPlanetShesternaBlur mcPlanetShesternaBlur = StaticPool.New<McPlanetShesternaBlur>();
            mcPlanetShesternaBlur.RefreshProperties();
            return mcPlanetShesternaBlur;
        }

        public McPlanetShesternaBlur()
            : base("planets/McPlanetShesternaBlur")
        {
        }

        public void Free()
        {
            StaticPool.Free<McPlanetShesternaBlur>(this);
        }
    }
}
