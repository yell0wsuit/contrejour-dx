using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McMultitouch2HintView : Sprite, IFreeable, IId
{
    public const string ID = "chapter1/McMultitouch2HintView";

    public string Id => "chapter1/McMultitouch2HintView";

    public static McMultitouch2HintView New()
    {
        McMultitouch2HintView mcMultitouch2HintView = StaticPool.New<McMultitouch2HintView>();
        mcMultitouch2HintView.RefreshProperties();
        return mcMultitouch2HintView;
    }

    public McMultitouch2HintView()
        : base("chapter1/McMultitouch2HintView")
    {
    }

    public void Free()
    {
        StaticPool.Free<McMultitouch2HintView>(this);
    }
}
