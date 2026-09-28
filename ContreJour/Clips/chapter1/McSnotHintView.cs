using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSnotHintView : Sprite, IFreeable, IId
{
    public const string ID = "chapter1/McSnotHintView";

    public string Id => "chapter1/McSnotHintView";

    public static McSnotHintView New()
    {
        McSnotHintView mcSnotHintView = StaticPool<McSnotHintView>.New();
        mcSnotHintView.RefreshProperties();
        return mcSnotHintView;
    }

    public McSnotHintView()
        : base("chapter1/McSnotHintView")
    {
    }

    public void Free()
    {
        StaticPool<McSnotHintView>.Free(this);
    }
}
