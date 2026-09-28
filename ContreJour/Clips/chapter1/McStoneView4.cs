using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McStoneView4 : Sprite, IFreeable, IId
{
    public const string ID = "chapter1/McStoneView4";

    public string Id => "chapter1/McStoneView4";

    public static McStoneView4 New()
    {
        McStoneView4 mcStoneView = StaticPool<McStoneView4>.New();
        mcStoneView.RefreshProperties();
        return mcStoneView;
    }

    public McStoneView4()
        : base("chapter1/McStoneView4")
    {
    }

    public void Free()
    {
        StaticPool<McStoneView4>.Free(this);
    }
}
