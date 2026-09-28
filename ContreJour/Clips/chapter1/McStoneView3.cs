using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter1;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McStoneView3 : Sprite, IFreeable, IId
{
    public const string ID = "chapter1/McStoneView3";

    public string Id => "chapter1/McStoneView3";

    public static McStoneView3 New()
    {
        McStoneView3 mcStoneView = StaticPool.New<McStoneView3>();
        mcStoneView.RefreshProperties();
        return mcStoneView;
    }

    public McStoneView3()
        : base("chapter1/McStoneView3")
    {
    }

    public void Free()
    {
        StaticPool.Free<McStoneView3>(this);
    }
}
