using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McHeroHighliteMenu : Sprite, IFreeable, IId
{
    public const string ID = "menu/McHeroHighliteMenu";

    public string Id => "menu/McHeroHighliteMenu";

    public static McHeroHighliteMenu New()
    {
        McHeroHighliteMenu mcHeroHighliteMenu = StaticPool.New<McHeroHighliteMenu>();
        mcHeroHighliteMenu.RefreshProperties();
        return mcHeroHighliteMenu;
    }

    public McHeroHighliteMenu()
        : base("menu/McHeroHighliteMenu")
    {
    }

    public void Free()
    {
        StaticPool.Free<McHeroHighliteMenu>(this);
    }
}
