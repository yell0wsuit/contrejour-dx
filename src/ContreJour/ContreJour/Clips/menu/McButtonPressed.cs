using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McButtonPressed : Sprite, IFreeable, IId
{
    public const string ID = "menu/McButtonPressed";

    public string Id => "menu/McButtonPressed";

    public static McButtonPressed New()
    {
        McButtonPressed mcButtonPressed = StaticPool.New<McButtonPressed>();
        mcButtonPressed.RefreshProperties();
        return mcButtonPressed;
    }

    public McButtonPressed()
        : base("menu/McButtonPressed")
    {
    }

    public void Free()
    {
        StaticPool.Free<McButtonPressed>(this);
    }
}
