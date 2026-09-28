using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEggView0 : Sprite, IFreeable, IId
{
    public const string ID = "chapter4/McEggView0";

    public string Id => "chapter4/McEggView0";

    public static McEggView0 New()
    {
        McEggView0 mcEggView = StaticPool.New<McEggView0>();
        mcEggView.RefreshProperties();
        return mcEggView;
    }

    public McEggView0()
        : base("chapter4/McEggView0")
    {
    }

    public void Free()
    {
        StaticPool.Free<McEggView0>(this);
    }
}
