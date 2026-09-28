using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEggView2_5 : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McEggView2_5";

    public string Id => "chapter5/McEggView2_5";

    public static McEggView2_5 New()
    {
        McEggView2_5 mcEggView2_ = StaticPool.New<McEggView2_5>();
        mcEggView2_.RefreshProperties();
        return mcEggView2_;
    }

    public McEggView2_5()
        : base("chapter5/McEggView2_5")
    {
    }

    public void Free()
    {
        StaticPool.Free<McEggView2_5>(this);
    }
}
