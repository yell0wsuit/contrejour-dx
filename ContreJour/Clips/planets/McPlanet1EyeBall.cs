using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanet1EyeBall : Sprite, IFreeable, IId
{
    public const string ID = "planets/McPlanet1EyeBall";

    public string Id => "planets/McPlanet1EyeBall";

    public static McPlanet1EyeBall New()
    {
        McPlanet1EyeBall mcPlanet1EyeBall = StaticPool<McPlanet1EyeBall>.New();
        mcPlanet1EyeBall.RefreshProperties();
        return mcPlanet1EyeBall;
    }

    public McPlanet1EyeBall()
        : base("planets/McPlanet1EyeBall")
    {
    }

    public void Free()
    {
        StaticPool<McPlanet1EyeBall>.Free(this);
    }
}
