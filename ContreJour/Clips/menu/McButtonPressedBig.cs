using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McButtonPressedBig : Sprite, IFreeable, IId
{
    public const string ID = "menu/McButtonPressedBig";

    public string Id => "menu/McButtonPressedBig";

    public static McButtonPressedBig New()
    {
        McButtonPressedBig mcButtonPressedBig = StaticPool<McButtonPressedBig>.New();
        mcButtonPressedBig.RefreshProperties();
        return mcButtonPressedBig;
    }

    public McButtonPressedBig()
        : base("menu/McButtonPressedBig")
    {
    }

    public void Free()
    {
        StaticPool<McButtonPressedBig>.Free(this);
    }
}
