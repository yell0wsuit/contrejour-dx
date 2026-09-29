using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBridgeHideHintView : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McBridgeHideHintView";

    public string Id => "chapter5/McBridgeHideHintView";

    public static McBridgeHideHintView New()
    {
        McBridgeHideHintView mcBridgeHideHintView = StaticPool.New<McBridgeHideHintView>();
        mcBridgeHideHintView.RefreshProperties();
        return mcBridgeHideHintView;
    }

    public McBridgeHideHintView()
        : base("chapter5/McBridgeHideHintView")
    {
    }

    public void Free()
    {
        StaticPool.Free<McBridgeHideHintView>(this);
    }
}
