using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McBigButtonBackground : Sprite, IFreeable, IId
{
    public const string ID = "menu/McBigButtonBackground";

    public string Id => "menu/McBigButtonBackground";

    public static McBigButtonBackground New()
    {
        McBigButtonBackground mcBigButtonBackground = StaticPool<McBigButtonBackground>.New();
        mcBigButtonBackground.RefreshProperties();
        return mcBigButtonBackground;
    }

    public McBigButtonBackground()
        : base("menu/McBigButtonBackground")
    {
    }

    public void Free()
    {
        StaticPool<McBigButtonBackground>.Free(this);
    }
}
