using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanetStickBall : Sprite, IFreeable, IId
{
    public const string ID = "planets/McPlanetStickBall";

    public string Id => "planets/McPlanetStickBall";

    public static McPlanetStickBall New()
    {
        McPlanetStickBall mcPlanetStickBall = StaticPool.New<McPlanetStickBall>();
        mcPlanetStickBall.RefreshProperties();
        return mcPlanetStickBall;
    }

    public McPlanetStickBall()
        : base("planets/McPlanetStickBall")
    {
    }

    public void Free()
    {
        StaticPool.Free<McPlanetStickBall>(this);
    }
}
