using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEggView1 : Sprite, IFreeable, IId
{
    public const string ID = "chapter4/McEggView1";

    public string Id => "chapter4/McEggView1";

    public static McEggView1 New()
    {
        McEggView1 mcEggView = StaticPool.New<McEggView1>();
        mcEggView.RefreshProperties();
        return mcEggView;
    }

    public McEggView1()
        : base("chapter4/McEggView1")
    {
    }

    public void Free()
    {
        StaticPool.Free<McEggView1>(this);
    }
}
