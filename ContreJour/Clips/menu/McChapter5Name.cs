using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McChapter5Name : Sprite, IFreeable, IId
{
    public const string ID = "menu/McChapter5Name";

    public string Id => "menu/McChapter5Name";

    public static McChapter5Name New()
    {
        McChapter5Name mcChapter5Name = StaticPool<McChapter5Name>.New();
        mcChapter5Name.RefreshProperties();
        return mcChapter5Name;
    }

    public McChapter5Name()
        : base("menu/McChapter5Name")
    {
    }

    public void Free()
    {
        StaticPool<McChapter5Name>.Free(this);
    }
}
