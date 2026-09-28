using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanetShesterna3 : Sprite, IFreeable, IId
{
    public const string ID = "planets/McPlanetShesterna3";

    public string Id => "planets/McPlanetShesterna3";

    public static McPlanetShesterna3 New()
    {
        McPlanetShesterna3 mcPlanetShesterna = StaticPool.New<McPlanetShesterna3>();
        mcPlanetShesterna.RefreshProperties();
        return mcPlanetShesterna;
    }

    public McPlanetShesterna3()
        : base("planets/McPlanetShesterna3")
    {
    }

    public void Free()
    {
        StaticPool.Free<McPlanetShesterna3>(this);
    }
}
