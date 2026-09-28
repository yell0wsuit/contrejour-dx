using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McChapter5MenuBackground : Sprite, IFreeable, IId
{
    public const string ID = "menu2/McChapter5MenuBackground";

    public string Id => "menu2/McChapter5MenuBackground";

    public static McChapter5MenuBackground New()
    {
        McChapter5MenuBackground mcChapter5MenuBackground = StaticPool<McChapter5MenuBackground>.New();
        mcChapter5MenuBackground.RefreshProperties();
        return mcChapter5MenuBackground;
    }

    public McChapter5MenuBackground()
        : base("menu2/McChapter5MenuBackground")
    {
    }

    public void Free()
    {
        StaticPool<McChapter5MenuBackground>.Free(this);
    }
}
