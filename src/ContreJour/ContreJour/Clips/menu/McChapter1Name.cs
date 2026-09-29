using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McChapter1Name : Sprite, IFreeable, IId
{
    public const string ID = "menu/McChapter1Name";

    public string Id => "menu/McChapter1Name";

    public static McChapter1Name New()
    {
        McChapter1Name mcChapter1Name = StaticPool.New<McChapter1Name>();
        mcChapter1Name.RefreshProperties();
        return mcChapter1Name;
    }

    public McChapter1Name()
        : base("menu/McChapter1Name")
    {
    }

    public void Free()
    {
        StaticPool.Free<McChapter1Name>(this);
    }
}
