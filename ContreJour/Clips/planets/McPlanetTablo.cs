using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanetTablo : Sprite, IFreeable, IId
{
    public const string ID = "planets/McPlanetTablo";

    public string Id => "planets/McPlanetTablo";

    public static McPlanetTablo New()
    {
        McPlanetTablo mcPlanetTablo = StaticPool<McPlanetTablo>.New();
        mcPlanetTablo.RefreshProperties();
        return mcPlanetTablo;
    }

    public McPlanetTablo()
        : base("planets/McPlanetTablo")
    {
    }

    public void Free()
    {
        StaticPool<McPlanetTablo>.Free(this);
    }
}
