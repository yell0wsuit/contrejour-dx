using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McChapter4MenuBackground : Sprite, IFreeable, IId
{
    public const string ID = "menu2/McChapter4MenuBackground";

    public string Id => "menu2/McChapter4MenuBackground";

    public static McChapter4MenuBackground New()
    {
        McChapter4MenuBackground mcChapter4MenuBackground = StaticPool<McChapter4MenuBackground>.New();
        mcChapter4MenuBackground.RefreshProperties();
        return mcChapter4MenuBackground;
    }

    public McChapter4MenuBackground()
        : base("menu2/McChapter4MenuBackground")
    {
    }

    public void Free()
    {
        StaticPool<McChapter4MenuBackground>.Free(this);
    }
}
