using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.common
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McEggBridgeView : Sprite, IFreeable, IId
    {
        public const string ID = "common/McEggBridgeView";

        public string Id => "common/McEggBridgeView";

        public static McEggBridgeView New()
        {
            McEggBridgeView mcEggBridgeView = StaticPool.New<McEggBridgeView>();
            mcEggBridgeView.RefreshProperties();
            return mcEggBridgeView;
        }

        public McEggBridgeView()
            : base("common/McEggBridgeView")
        {
        }

        public void Free()
        {
            StaticPool.Free<McEggBridgeView>(this);
        }
    }
}
