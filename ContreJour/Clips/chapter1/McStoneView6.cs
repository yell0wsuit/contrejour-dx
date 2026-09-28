using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McStoneView6 : Sprite, IFreeable, IId
{
    public const string ID = "chapter1/McStoneView6";

    public string Id => "chapter1/McStoneView6";

    public static McStoneView6 New()
    {
        McStoneView6 mcStoneView = StaticPool<McStoneView6>.New();
        mcStoneView.RefreshProperties();
        return mcStoneView;
    }

    public McStoneView6()
        : base("chapter1/McStoneView6")
    {
    }

    public void Free()
    {
        StaticPool<McStoneView6>.Free(this);
    }
}
