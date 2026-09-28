using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSlingshotHintView : Sprite, IFreeable, IId
{
    public const string ID = "chapter1/McSlingshotHintView";

    public string Id => "chapter1/McSlingshotHintView";

    public static McSlingshotHintView New()
    {
        McSlingshotHintView mcSlingshotHintView = StaticPool<McSlingshotHintView>.New();
        mcSlingshotHintView.RefreshProperties();
        return mcSlingshotHintView;
    }

    public McSlingshotHintView()
        : base("chapter1/McSlingshotHintView")
    {
    }

    public void Free()
    {
        StaticPool<McSlingshotHintView>.Free(this);
    }
}
