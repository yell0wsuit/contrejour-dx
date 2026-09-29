using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRetryHintView : Sprite, IFreeable, IId
{
    public const string ID = "chapter1/McRetryHintView";

    public string Id => "chapter1/McRetryHintView";

    public static McRetryHintView New()
    {
        McRetryHintView mcRetryHintView = StaticPool.New<McRetryHintView>();
        mcRetryHintView.RefreshProperties();
        return mcRetryHintView;
    }

    public McRetryHintView()
        : base("chapter1/McRetryHintView")
    {
    }

    public void Free()
    {
        StaticPool.Free<McRetryHintView>(this);
    }
}
