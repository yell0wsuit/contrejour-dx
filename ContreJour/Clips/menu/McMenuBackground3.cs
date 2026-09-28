using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McMenuBackground3 : Sprite, IFreeable, IId
{
    public const string ID = "menu/McMenuBackground3";

    public string Id => "menu/McMenuBackground3";

    public static McMenuBackground3 New()
    {
        McMenuBackground3 mcMenuBackground = StaticPool<McMenuBackground3>.New();
        mcMenuBackground.RefreshProperties();
        return mcMenuBackground;
    }

    public McMenuBackground3()
        : base("menu/McMenuBackground3")
    {
    }

    public void Free()
    {
        StaticPool<McMenuBackground3>.Free(this);
    }
}
