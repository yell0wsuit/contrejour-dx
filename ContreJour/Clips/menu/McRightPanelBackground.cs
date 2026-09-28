using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McRightPanelBackground : Sprite, IFreeable, IId
{
    public const string ID = "menu/McRightPanelBackground";

    public string Id => "menu/McRightPanelBackground";

    public static McRightPanelBackground New()
    {
        McRightPanelBackground mcRightPanelBackground = StaticPool<McRightPanelBackground>.New();
        mcRightPanelBackground.RefreshProperties();
        return mcRightPanelBackground;
    }

    public McRightPanelBackground()
        : base("menu/McRightPanelBackground")
    {
    }

    public void Free()
    {
        StaticPool<McRightPanelBackground>.Free(this);
    }
}
