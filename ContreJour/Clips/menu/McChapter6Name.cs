using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McChapter6Name : Sprite, IFreeable, IId
{
    public const string ID = "menu/McChapter6Name";

    public string Id => "menu/McChapter6Name";

    public static McChapter6Name New()
    {
        McChapter6Name mcChapter6Name = StaticPool<McChapter6Name>.New();
        mcChapter6Name.RefreshProperties();
        return mcChapter6Name;
    }

    public McChapter6Name()
        : base("menu/McChapter6Name")
    {
    }

    public void Free()
    {
        StaticPool<McChapter6Name>.Free(this);
    }
}
