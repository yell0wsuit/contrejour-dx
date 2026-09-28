using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McStoneView2 : Sprite, IFreeable, IId
{
    public const string ID = "chapter1/McStoneView2";

    public string Id => "chapter1/McStoneView2";

    public static McStoneView2 New()
    {
        McStoneView2 mcStoneView = StaticPool<McStoneView2>.New();
        mcStoneView.RefreshProperties();
        return mcStoneView;
    }

    public McStoneView2()
        : base("chapter1/McStoneView2")
    {
    }

    public void Free()
    {
        StaticPool<McStoneView2>.Free(this);
    }
}
