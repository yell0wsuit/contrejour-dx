using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanetShesterna : Sprite, IFreeable, IId
{
    public const string ID = "planets/McPlanetShesterna";

    public string Id => "planets/McPlanetShesterna";

    public static McPlanetShesterna New()
    {
        McPlanetShesterna mcPlanetShesterna = StaticPool<McPlanetShesterna>.New();
        mcPlanetShesterna.RefreshProperties();
        return mcPlanetShesterna;
    }

    public McPlanetShesterna()
        : base("planets/McPlanetShesterna")
    {
    }

    public void Free()
    {
        StaticPool<McPlanetShesterna>.Free(this);
    }
}
