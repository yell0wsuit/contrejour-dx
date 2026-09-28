using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McGetMoreLevelsButton : Sprite, IFreeable, IId
{
    public const string ID = "menu/McGetMoreLevelsButton";

    public string Id => "menu/McGetMoreLevelsButton";

    public static McGetMoreLevelsButton New()
    {
        McGetMoreLevelsButton mcGetMoreLevelsButton = StaticPool<McGetMoreLevelsButton>.New();
        mcGetMoreLevelsButton.RefreshProperties();
        return mcGetMoreLevelsButton;
    }

    public McGetMoreLevelsButton()
        : base("menu/McGetMoreLevelsButton")
    {
    }

    public void Free()
    {
        StaticPool<McGetMoreLevelsButton>.Free(this);
    }
}
