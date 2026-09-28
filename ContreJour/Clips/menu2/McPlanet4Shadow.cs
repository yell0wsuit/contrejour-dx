using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanet4Shadow : Sprite, IFreeable, IId
{
    public const string ID = "menu2/McPlanet4Shadow";

    public string Id => "menu2/McPlanet4Shadow";

    public static McPlanet4Shadow New()
    {
        McPlanet4Shadow mcPlanet4Shadow = StaticPool.New<McPlanet4Shadow>();
        mcPlanet4Shadow.RefreshProperties();
        return mcPlanet4Shadow;
    }

    public McPlanet4Shadow()
        : base("menu2/McPlanet4Shadow")
    {
    }

    public void Free()
    {
        StaticPool.Free<McPlanet4Shadow>(this);
    }
}
