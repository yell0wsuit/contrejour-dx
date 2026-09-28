using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanetEye : Sprite, IFreeable, IId
{
    public const string ID = "planets/McPlanetEye";

    public string Id => "planets/McPlanetEye";

    public static McPlanetEye New()
    {
        McPlanetEye mcPlanetEye = StaticPool<McPlanetEye>.New();
        mcPlanetEye.RefreshProperties();
        return mcPlanetEye;
    }

    public McPlanetEye()
        : base("planets/McPlanetEye")
    {
    }

    public void Free()
    {
        StaticPool<McPlanetEye>.Free(this);
    }
}
