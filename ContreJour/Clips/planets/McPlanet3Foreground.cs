using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanet3Foreground : Sprite, IFreeable, IId
{
    public const string ID = "planets/McPlanet3Foreground";

    public string Id => "planets/McPlanet3Foreground";

    public static McPlanet3Foreground New()
    {
        McPlanet3Foreground mcPlanet3Foreground = StaticPool<McPlanet3Foreground>.New();
        mcPlanet3Foreground.RefreshProperties();
        return mcPlanet3Foreground;
    }

    public McPlanet3Foreground()
        : base("planets/McPlanet3Foreground")
    {
    }

    public void Free()
    {
        StaticPool<McPlanet3Foreground>.Free(this);
    }
}
