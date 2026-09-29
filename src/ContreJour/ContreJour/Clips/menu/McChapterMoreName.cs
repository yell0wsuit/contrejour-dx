using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McChapterMoreName : Sprite, IFreeable, IId
{
    public const string ID = "menu/McChapterMoreName";

    public string Id => "menu/McChapterMoreName";

    public static McChapterMoreName New()
    {
        McChapterMoreName mcChapterMoreName = StaticPool.New<McChapterMoreName>();
        mcChapterMoreName.RefreshProperties();
        return mcChapterMoreName;
    }

    public McChapterMoreName()
        : base("menu/McChapterMoreName")
    {
    }

    public void Free()
    {
        StaticPool.Free<McChapterMoreName>(this);
    }
}
