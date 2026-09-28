using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McGetUniversalButton : Sprite, IFreeable, IId
{
    public const string ID = "menu/McGetUniversalButton";

    public string Id => "menu/McGetUniversalButton";

    public static McGetUniversalButton New()
    {
        McGetUniversalButton mcGetUniversalButton = StaticPool<McGetUniversalButton>.New();
        mcGetUniversalButton.RefreshProperties();
        return mcGetUniversalButton;
    }

    public McGetUniversalButton()
        : base("menu/McGetUniversalButton")
    {
    }

    public void Free()
    {
        StaticPool<McGetUniversalButton>.Free(this);
    }
}
