using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McChapter2Name : Sprite, IFreeable, IId
{
    public const string ID = "menu/McChapter2Name";

    public string Id => "menu/McChapter2Name";

    public static McChapter2Name New()
    {
        McChapter2Name mcChapter2Name = StaticPool<McChapter2Name>.New();
        mcChapter2Name.RefreshProperties();
        return mcChapter2Name;
    }

    public McChapter2Name()
        : base("menu/McChapter2Name")
    {
    }

    public void Free()
    {
        StaticPool<McChapter2Name>.Free(this);
    }
}
