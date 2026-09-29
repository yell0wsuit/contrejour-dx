using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5
{
    [GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
    public class McBridgeHintView : Sprite, IFreeable, IId
    {
        public const string ID = "chapter5/McBridgeHintView";

        public string Id => "chapter5/McBridgeHintView";

        public static McBridgeHintView New()
        {
            McBridgeHintView mcBridgeHintView = StaticPool.New<McBridgeHintView>();
            mcBridgeHintView.RefreshProperties();
            return mcBridgeHintView;
        }

        public McBridgeHintView()
            : base("chapter5/McBridgeHintView")
        {
        }

        public void Free()
        {
            StaticPool.Free<McBridgeHintView>(this);
        }
    }
}
