using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPinchHintView : Sprite, IFreeable, IId
{
    public const string ID = "chapter1/McPinchHintView";

    public string Id => "chapter1/McPinchHintView";

    public static McPinchHintView New()
    {
        McPinchHintView mcPinchHintView = StaticPool<McPinchHintView>.New();
        mcPinchHintView.RefreshProperties();
        return mcPinchHintView;
    }

    public McPinchHintView()
        : base("chapter1/McPinchHintView")
    {
    }

    public void Free()
    {
        StaticPool<McPinchHintView>.Free(this);
    }
}
