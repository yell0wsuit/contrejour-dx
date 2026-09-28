using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.chapter2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBackgroundStoneView1Black : Sprite, IFreeable, IId
{
    public const string ID = "chapter2/McBackgroundStoneView1Black";

    public string Id => "chapter2/McBackgroundStoneView1Black";

    public static McBackgroundStoneView1Black New()
    {
        McBackgroundStoneView1Black mcBackgroundStoneView1Black = StaticPool<McBackgroundStoneView1Black>.New();
        mcBackgroundStoneView1Black.RefreshProperties();
        return mcBackgroundStoneView1Black;
    }

    public McBackgroundStoneView1Black()
        : base("chapter2/McBackgroundStoneView1Black")
    {
    }

    public void Free()
    {
        StaticPool<McBackgroundStoneView1Black>.Free(this);
    }
}
