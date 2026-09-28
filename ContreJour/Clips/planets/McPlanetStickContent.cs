using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanetStickContent : Sprite, IFreeable, IId
{
    public const string ID = "planets/McPlanetStickContent";

    public string Id => "planets/McPlanetStickContent";

    public static McPlanetStickContent New()
    {
        McPlanetStickContent mcPlanetStickContent = StaticPool<McPlanetStickContent>.New();
        mcPlanetStickContent.RefreshProperties();
        return mcPlanetStickContent;
    }

    public McPlanetStickContent()
        : base("planets/McPlanetStickContent")
    {
    }

    public void Free()
    {
        StaticPool<McPlanetStickContent>.Free(this);
    }
}
