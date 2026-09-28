using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanet1Background : Sprite, IFreeable, IId
{
    public const string ID = "planets/McPlanet1Background";

    public string Id => "planets/McPlanet1Background";

    public static McPlanet1Background New()
    {
        McPlanet1Background mcPlanet1Background = StaticPool.New<McPlanet1Background>();
        mcPlanet1Background.RefreshProperties();
        return mcPlanet1Background;
    }

    public McPlanet1Background()
        : base("planets/McPlanet1Background")
    {
    }

    public void Free()
    {
        StaticPool.Free<McPlanet1Background>(this);
    }
}
