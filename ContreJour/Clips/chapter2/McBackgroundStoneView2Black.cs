using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackgroundStoneView2Black : Sprite, IFreeable, IId
{
    public const string ID = "chapter2/McBackgroundStoneView2Black";

    public string Id => "chapter2/McBackgroundStoneView2Black";

    public static McBackgroundStoneView2Black New()
    {
        McBackgroundStoneView2Black mcBackgroundStoneView2Black = StaticPool.New<McBackgroundStoneView2Black>();
        mcBackgroundStoneView2Black.RefreshProperties();
        return mcBackgroundStoneView2Black;
    }

    public McBackgroundStoneView2Black()
        : base("chapter2/McBackgroundStoneView2Black")
    {
    }

    public void Free()
    {
        StaticPool.Free<McBackgroundStoneView2Black>(this);
    }
}
