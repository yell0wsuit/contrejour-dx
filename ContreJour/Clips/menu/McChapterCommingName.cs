using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McChapterCommingName : Sprite, IFreeable, IId
{
    public const string ID = "menu/McChapterCommingName";

    public string Id => "menu/McChapterCommingName";

    public static McChapterCommingName New()
    {
        McChapterCommingName mcChapterCommingName = StaticPool.New<McChapterCommingName>();
        mcChapterCommingName.RefreshProperties();
        return mcChapterCommingName;
    }

    public McChapterCommingName()
        : base("menu/McChapterCommingName")
    {
    }

    public void Free()
    {
        StaticPool.Free<McChapterCommingName>(this);
    }
}
