using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanet2Background : Sprite, IFreeable, IId
{
    public const string ID = "planets/McPlanet2Background";

    public string Id => "planets/McPlanet2Background";

    public static McPlanet2Background New()
    {
        McPlanet2Background mcPlanet2Background = StaticPool.New<McPlanet2Background>();
        mcPlanet2Background.RefreshProperties();
        return mcPlanet2Background;
    }

    public McPlanet2Background()
        : base("planets/McPlanet2Background")
    {
    }

    public void Free()
    {
        StaticPool.Free<McPlanet2Background>(this);
    }
}
