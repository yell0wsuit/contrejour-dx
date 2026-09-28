using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSkipHintView : Sprite, IFreeable, IId
{
    public const string ID = "chapter1/McSkipHintView";

    public string Id => "chapter1/McSkipHintView";

    public static McSkipHintView New()
    {
        McSkipHintView mcSkipHintView = StaticPool<McSkipHintView>.New();
        mcSkipHintView.RefreshProperties();
        return mcSkipHintView;
    }

    public McSkipHintView()
        : base("chapter1/McSkipHintView")
    {
    }

    public void Free()
    {
        StaticPool<McSkipHintView>.Free(this);
    }
}
