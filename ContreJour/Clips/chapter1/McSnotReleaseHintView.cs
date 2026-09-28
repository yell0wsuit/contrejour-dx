using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSnotReleaseHintView : Sprite, IFreeable, IId
{
    public const string ID = "chapter1/McSnotReleaseHintView";

    public string Id => "chapter1/McSnotReleaseHintView";

    public static McSnotReleaseHintView New()
    {
        McSnotReleaseHintView mcSnotReleaseHintView = StaticPool.New<McSnotReleaseHintView>();
        mcSnotReleaseHintView.RefreshProperties();
        return mcSnotReleaseHintView;
    }

    public McSnotReleaseHintView()
        : base("chapter1/McSnotReleaseHintView")
    {
    }

    public void Free()
    {
        StaticPool.Free<McSnotReleaseHintView>(this);
    }
}
