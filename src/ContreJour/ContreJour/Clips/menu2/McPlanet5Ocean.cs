using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanet5Ocean : Sprite, IFreeable, IId
{
    public const string ID = "menu2/McPlanet5Ocean";

    public string Id => "menu2/McPlanet5Ocean";

    public static McPlanet5Ocean New()
    {
        McPlanet5Ocean mcPlanet5Ocean = StaticPool.New<McPlanet5Ocean>();
        mcPlanet5Ocean.RefreshProperties();
        return mcPlanet5Ocean;
    }

    public McPlanet5Ocean()
        : base("menu2/McPlanet5Ocean")
    {
    }

    public void Free()
    {
        StaticPool.Free<McPlanet5Ocean>(this);
    }
}
