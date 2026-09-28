using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McStoneView5 : Sprite, IFreeable, IId
{
    public const string ID = "chapter1/McStoneView5";

    public string Id => "chapter1/McStoneView5";

    public static McStoneView5 New()
    {
        McStoneView5 mcStoneView = StaticPool<McStoneView5>.New();
        mcStoneView.RefreshProperties();
        return mcStoneView;
    }

    public McStoneView5()
        : base("chapter1/McStoneView5")
    {
    }

    public void Free()
    {
        StaticPool<McStoneView5>.Free(this);
    }
}
