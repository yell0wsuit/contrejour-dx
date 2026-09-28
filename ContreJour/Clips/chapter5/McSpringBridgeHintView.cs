using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSpringBridgeHintView : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McSpringBridgeHintView";

    public string Id => "chapter5/McSpringBridgeHintView";

    public static McSpringBridgeHintView New()
    {
        McSpringBridgeHintView mcSpringBridgeHintView = StaticPool<McSpringBridgeHintView>.New();
        mcSpringBridgeHintView.RefreshProperties();
        return mcSpringBridgeHintView;
    }

    public McSpringBridgeHintView()
        : base("chapter5/McSpringBridgeHintView")
    {
    }

    public void Free()
    {
        StaticPool<McSpringBridgeHintView>.Free(this);
    }
}
