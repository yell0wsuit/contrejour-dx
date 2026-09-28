using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRotatorHintView : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McRotatorHintView";

    public string Id => "chapter5/McRotatorHintView";

    public static McRotatorHintView New()
    {
        McRotatorHintView mcRotatorHintView = StaticPool.New<McRotatorHintView>();
        mcRotatorHintView.RefreshProperties();
        return mcRotatorHintView;
    }

    public McRotatorHintView()
        : base("chapter5/McRotatorHintView")
    {
    }

    public void Free()
    {
        StaticPool.Free<McRotatorHintView>(this);
    }
}
