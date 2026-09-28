using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McGroundHintView : Sprite, IFreeable, IId
{
    public const string ID = "chapter1/McGroundHintView";

    public string Id => "chapter1/McGroundHintView";

    public static McGroundHintView New()
    {
        McGroundHintView mcGroundHintView = StaticPool.New<McGroundHintView>();
        mcGroundHintView.RefreshProperties();
        return mcGroundHintView;
    }

    public McGroundHintView()
        : base("chapter1/McGroundHintView")
    {
    }

    public void Free()
    {
        StaticPool.Free<McGroundHintView>(this);
    }
}
