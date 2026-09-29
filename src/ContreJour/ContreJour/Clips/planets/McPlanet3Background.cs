using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanet3Background : Sprite, IFreeable, IId
{
    public const string ID = "planets/McPlanet3Background";

    public string Id => "planets/McPlanet3Background";

    public static McPlanet3Background New()
    {
        McPlanet3Background mcPlanet3Background = StaticPool.New<McPlanet3Background>();
        mcPlanet3Background.RefreshProperties();
        return mcPlanet3Background;
    }

    public McPlanet3Background()
        : base("planets/McPlanet3Background")
    {
    }

    public void Free()
    {
        StaticPool.Free<McPlanet3Background>(this);
    }
}
