using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter4;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEggView3 : Sprite, IFreeable, IId
{
    public const string ID = "chapter4/McEggView3";

    public string Id => "chapter4/McEggView3";

    public static McEggView3 New()
    {
        McEggView3 mcEggView = StaticPool.New<McEggView3>();
        mcEggView.RefreshProperties();
        return mcEggView;
    }

    public McEggView3()
        : base("chapter4/McEggView3")
    {
    }

    public void Free()
    {
        StaticPool.Free<McEggView3>(this);
    }
}
