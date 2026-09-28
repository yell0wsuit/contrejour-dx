using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanet1Foreground : Sprite, IFreeable, IId
{
    public const string ID = "planets/McPlanet1Foreground";

    public string Id => "planets/McPlanet1Foreground";

    public static McPlanet1Foreground New()
    {
        McPlanet1Foreground mcPlanet1Foreground = StaticPool<McPlanet1Foreground>.New();
        mcPlanet1Foreground.RefreshProperties();
        return mcPlanet1Foreground;
    }

    public McPlanet1Foreground()
        : base("planets/McPlanet1Foreground")
    {
    }

    public void Free()
    {
        StaticPool<McPlanet1Foreground>.Free(this);
    }
}
