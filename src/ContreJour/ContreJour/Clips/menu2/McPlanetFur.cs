using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanetFur : MovieClip, IFreeable, IId
{
    public const string ID = "menu2/McPlanetFur";

    public string Id => "menu2/McPlanetFur";

    public static McPlanetFur New()
    {
        McPlanetFur mcPlanetFur = StaticPool.New<McPlanetFur>();
        mcPlanetFur.RefreshProperties();
        return mcPlanetFur;
    }

    public McPlanetFur()
        : base("menu2/McPlanetFur")
    {
    }

    public void Free()
    {
        StaticPool.Free<McPlanetFur>(this);
    }
}
