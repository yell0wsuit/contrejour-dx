using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanetEyeBlink : MovieClip, IFreeable, IId
{
    public const string ID = "planets/McPlanetEyeBlink";

    public string Id => "planets/McPlanetEyeBlink";

    public static McPlanetEyeBlink New()
    {
        McPlanetEyeBlink mcPlanetEyeBlink = StaticPool<McPlanetEyeBlink>.New();
        mcPlanetEyeBlink.RefreshProperties();
        return mcPlanetEyeBlink;
    }

    public McPlanetEyeBlink()
        : base("planets/McPlanetEyeBlink")
    {
    }

    public void Free()
    {
        StaticPool<McPlanetEyeBlink>.Free(this);
    }
}
