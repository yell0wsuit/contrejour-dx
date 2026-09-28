using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McStoneView0 : Sprite, IFreeable, IId
{
    public const string ID = "chapter1/McStoneView0";

    public string Id => "chapter1/McStoneView0";

    public static McStoneView0 New()
    {
        McStoneView0 mcStoneView = StaticPool<McStoneView0>.New();
        mcStoneView.RefreshProperties();
        return mcStoneView;
    }

    public McStoneView0()
        : base("chapter1/McStoneView0")
    {
    }

    public void Free()
    {
        StaticPool<McStoneView0>.Free(this);
    }
}
