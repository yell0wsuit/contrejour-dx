using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McHotspotwhite : Sprite, IFreeable, IId
    {
        public const string ID = "common/McHotspotwhite";

        public string Id => "common/McHotspotwhite";

        public static McHotspotwhite New()
        {
            McHotspotwhite mcHotspotwhite = StaticPool.New<McHotspotwhite>();
            mcHotspotwhite.RefreshProperties();
            return mcHotspotwhite;
        }

        public McHotspotwhite()
            : base("common/McHotspotwhite")
        {
        }

        public void Free()
        {
            StaticPool.Free<McHotspotwhite>(this);
        }
    }
}
