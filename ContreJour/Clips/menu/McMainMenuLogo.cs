using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McMainMenuLogo : Sprite, IFreeable, IId
{
    public const string ID = "menu/McMainMenuLogo";

    public string Id => "menu/McMainMenuLogo";

    public static McMainMenuLogo New()
    {
        McMainMenuLogo mcMainMenuLogo = StaticPool<McMainMenuLogo>.New();
        mcMainMenuLogo.RefreshProperties();
        return mcMainMenuLogo;
    }

    public McMainMenuLogo()
        : base("menu/McMainMenuLogo")
    {
    }

    public void Free()
    {
        StaticPool<McMainMenuLogo>.Free(this);
    }
}
