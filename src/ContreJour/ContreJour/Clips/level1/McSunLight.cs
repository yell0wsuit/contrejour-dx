using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.level1
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McSunLight : Sprite, IFreeable, IId
    {
        public const string ID = "level1/McSunLight";

        public string Id => "level1/McSunLight";

        public static McSunLight New()
        {
            McSunLight mcSunLight = StaticPool.New<McSunLight>();
            mcSunLight.RefreshProperties();
            return mcSunLight;
        }

        public McSunLight()
            : base("level1/McSunLight")
        {
        }

        public void Free()
        {
            StaticPool.Free<McSunLight>(this);
        }
    }
}
