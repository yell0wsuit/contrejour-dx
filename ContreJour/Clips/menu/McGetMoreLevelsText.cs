using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McGetMoreLevelsText : Sprite, IFreeable, IId
{
    public const string ID = "menu/McGetMoreLevelsText";

    public string Id => "menu/McGetMoreLevelsText";

    public static McGetMoreLevelsText New()
    {
        McGetMoreLevelsText mcGetMoreLevelsText = StaticPool<McGetMoreLevelsText>.New();
        mcGetMoreLevelsText.RefreshProperties();
        return mcGetMoreLevelsText;
    }

    public McGetMoreLevelsText()
        : base("menu/McGetMoreLevelsText")
    {
    }

    public void Free()
    {
        StaticPool<McGetMoreLevelsText>.Free(this);
    }
}
