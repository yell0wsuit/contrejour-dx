using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEggView3_5 : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McEggView3_5";

    public string Id => "chapter5/McEggView3_5";

    public static McEggView3_5 New()
    {
        McEggView3_5 mcEggView3_ = StaticPool.New<McEggView3_5>();
        mcEggView3_.RefreshProperties();
        return mcEggView3_;
    }

    public McEggView3_5()
        : base("chapter5/McEggView3_5")
    {
    }

    public void Free()
    {
        StaticPool.Free<McEggView3_5>(this);
    }
}
