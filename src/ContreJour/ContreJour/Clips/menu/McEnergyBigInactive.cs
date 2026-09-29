using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McEnergyBigInactive : Sprite, IFreeable, IId
    {
        public const string ID = "menu/McEnergyBigInactive";

        public string Id => "menu/McEnergyBigInactive";

        public static McEnergyBigInactive New()
        {
            McEnergyBigInactive mcEnergyBigInactive = StaticPool.New<McEnergyBigInactive>();
            mcEnergyBigInactive.RefreshProperties();
            return mcEnergyBigInactive;
        }

        public McEnergyBigInactive()
            : base("menu/McEnergyBigInactive")
        {
        }

        public void Free()
        {
            StaticPool.Free<McEnergyBigInactive>(this);
        }
    }
}
