using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPortal2HintView : Sprite, IFreeable, IId
{
    public const string ID = "chapter2/McPortal2HintView";

    public string Id => "chapter2/McPortal2HintView";

    public static McPortal2HintView New()
    {
        McPortal2HintView mcPortal2HintView = StaticPool<McPortal2HintView>.New();
        mcPortal2HintView.RefreshProperties();
        return mcPortal2HintView;
    }

    public McPortal2HintView()
        : base("chapter2/McPortal2HintView")
    {
    }

    public void Free()
    {
        StaticPool<McPortal2HintView>.Free(this);
    }
}
