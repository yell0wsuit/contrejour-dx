using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McButtonBackgroundLight : Sprite, IFreeable, IId
{
    public const string ID = "menu/McButtonBackgroundLight";

    public string Id => "menu/McButtonBackgroundLight";

    public static McButtonBackgroundLight New()
    {
        McButtonBackgroundLight mcButtonBackgroundLight = StaticPool.New<McButtonBackgroundLight>();
        mcButtonBackgroundLight.RefreshProperties();
        return mcButtonBackgroundLight;
    }

    public McButtonBackgroundLight()
        : base("menu/McButtonBackgroundLight")
    {
    }

    public void Free()
    {
        StaticPool.Free<McButtonBackgroundLight>(this);
    }
}
