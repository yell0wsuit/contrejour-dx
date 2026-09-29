using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McEnergyBig : Sprite, IFreeable, IId
    {
        public const string ID = "menu/McEnergyBig";

        public string Id => "menu/McEnergyBig";

        public static McEnergyBig New()
        {
            McEnergyBig mcEnergyBig = StaticPool.New<McEnergyBig>();
            mcEnergyBig.RefreshProperties();
            return mcEnergyBig;
        }

        public McEnergyBig()
            : base("menu/McEnergyBig")
        {
        }

        public void Free()
        {
            StaticPool.Free<McEnergyBig>(this);
        }
    }
}
