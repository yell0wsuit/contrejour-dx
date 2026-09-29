using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McEnergyBall : Sprite, IFreeable, IId
    {
        public const string ID = "common/McEnergyBall";

        public string Id => "common/McEnergyBall";

        public static McEnergyBall New()
        {
            McEnergyBall mcEnergyBall = StaticPool.New<McEnergyBall>();
            mcEnergyBall.RefreshProperties();
            return mcEnergyBall;
        }

        public McEnergyBall()
            : base("common/McEnergyBall")
        {
        }

        public void Free()
        {
            StaticPool.Free<McEnergyBall>(this);
        }
    }
}
