using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEggBridgeView_5 : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McEggBridgeView_5";

    public string Id => "chapter5/McEggBridgeView_5";

    public static McEggBridgeView_5 New()
    {
        McEggBridgeView_5 mcEggBridgeView_ = StaticPool.New<McEggBridgeView_5>();
        mcEggBridgeView_.RefreshProperties();
        return mcEggBridgeView_;
    }

    public McEggBridgeView_5()
        : base("chapter5/McEggBridgeView_5")
    {
    }

    public void Free()
    {
        StaticPool.Free<McEggBridgeView_5>(this);
    }
}
