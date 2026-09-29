using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McChapter4Blur : Sprite, IFreeable, IId
{
    public const string ID = "menu2/McChapter4Blur";

    public string Id => "menu2/McChapter4Blur";

    public static McChapter4Blur New()
    {
        McChapter4Blur mcChapter4Blur = StaticPool.New<McChapter4Blur>();
        mcChapter4Blur.RefreshProperties();
        return mcChapter4Blur;
    }

    public McChapter4Blur()
        : base("menu2/McChapter4Blur")
    {
    }

    public void Free()
    {
        StaticPool.Free<McChapter4Blur>(this);
    }
}
