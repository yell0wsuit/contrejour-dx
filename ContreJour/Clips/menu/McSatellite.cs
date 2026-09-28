using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McSatellite : Sprite, IFreeable, IId
{
    public const string ID = "menu/McSatellite";

    public string Id => "menu/McSatellite";

    public static McSatellite New()
    {
        McSatellite mcSatellite = StaticPool.New<McSatellite>();
        mcSatellite.RefreshProperties();
        return mcSatellite;
    }

    public McSatellite()
        : base("menu/McSatellite")
    {
    }

    public void Free()
    {
        StaticPool.Free<McSatellite>(this);
    }
}
