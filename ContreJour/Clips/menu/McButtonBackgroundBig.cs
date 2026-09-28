using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McButtonBackgroundBig : Sprite, IFreeable, IId
{
    public const string ID = "menu/McButtonBackgroundBig";

    public string Id => "menu/McButtonBackgroundBig";

    public static McButtonBackgroundBig New()
    {
        McButtonBackgroundBig mcButtonBackgroundBig = StaticPool<McButtonBackgroundBig>.New();
        mcButtonBackgroundBig.RefreshProperties();
        return mcButtonBackgroundBig;
    }

    public McButtonBackgroundBig()
        : base("menu/McButtonBackgroundBig")
    {
    }

    public void Free()
    {
        StaticPool<McButtonBackgroundBig>.Free(this);
    }
}
