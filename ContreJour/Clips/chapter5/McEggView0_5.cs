using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter5;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEggView0_5 : Sprite, IFreeable, IId
{
    public const string ID = "chapter5/McEggView0_5";

    public string Id => "chapter5/McEggView0_5";

    public static McEggView0_5 New()
    {
        McEggView0_5 mcEggView0_ = StaticPool.New<McEggView0_5>();
        mcEggView0_.RefreshProperties();
        return mcEggView0_;
    }

    public McEggView0_5()
        : base("chapter5/McEggView0_5")
    {
    }

    public void Free()
    {
        StaticPool.Free<McEggView0_5>(this);
    }
}
