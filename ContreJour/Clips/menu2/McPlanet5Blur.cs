using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanet5Blur : Sprite, IFreeable, IId
{
    public const string ID = "menu2/McPlanet5Blur";

    public string Id => "menu2/McPlanet5Blur";

    public static McPlanet5Blur New()
    {
        McPlanet5Blur mcPlanet5Blur = StaticPool.New<McPlanet5Blur>();
        mcPlanet5Blur.RefreshProperties();
        return mcPlanet5Blur;
    }

    public McPlanet5Blur()
        : base("menu2/McPlanet5Blur")
    {
    }

    public void Free()
    {
        StaticPool.Free<McPlanet5Blur>(this);
    }
}
