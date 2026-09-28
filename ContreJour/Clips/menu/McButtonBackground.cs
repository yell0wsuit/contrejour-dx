using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McButtonBackground : Sprite, IFreeable, IId
{
    public const string ID = "menu/McButtonBackground";

    public string Id => "menu/McButtonBackground";

    public static McButtonBackground New()
    {
        McButtonBackground mcButtonBackground = StaticPool<McButtonBackground>.New();
        mcButtonBackground.RefreshProperties();
        return mcButtonBackground;
    }

    public McButtonBackground()
        : base("menu/McButtonBackground")
    {
    }

    public void Free()
    {
        StaticPool<McButtonBackground>.Free(this);
    }
}
