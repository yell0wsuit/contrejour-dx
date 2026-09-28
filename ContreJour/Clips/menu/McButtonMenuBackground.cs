using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McButtonMenuBackground : Sprite, IFreeable, IId
{
    public const string ID = "menu/McButtonMenuBackground";

    public string Id => "menu/McButtonMenuBackground";

    public static McButtonMenuBackground New()
    {
        McButtonMenuBackground mcButtonMenuBackground = StaticPool<McButtonMenuBackground>.New();
        mcButtonMenuBackground.RefreshProperties();
        return mcButtonMenuBackground;
    }

    public McButtonMenuBackground()
        : base("menu/McButtonMenuBackground")
    {
    }

    public void Free()
    {
        StaticPool<McButtonMenuBackground>.Free(this);
    }
}
