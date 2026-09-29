using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanetShesterna2 : Sprite, IFreeable, IId
{
    public const string ID = "planets/McPlanetShesterna2";

    public string Id => "planets/McPlanetShesterna2";

    public static McPlanetShesterna2 New()
    {
        McPlanetShesterna2 mcPlanetShesterna = StaticPool.New<McPlanetShesterna2>();
        mcPlanetShesterna.RefreshProperties();
        return mcPlanetShesterna;
    }

    public McPlanetShesterna2()
        : base("planets/McPlanetShesterna2")
    {
    }

    public void Free()
    {
        StaticPool.Free<McPlanetShesterna2>(this);
    }
}
