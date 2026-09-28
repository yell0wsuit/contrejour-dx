using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanet5Background : Sprite, IFreeable, IId
{
    public const string ID = "menu2/McPlanet5Background";

    public string Id => "menu2/McPlanet5Background";

    public static McPlanet5Background New()
    {
        McPlanet5Background mcPlanet5Background = StaticPool<McPlanet5Background>.New();
        mcPlanet5Background.RefreshProperties();
        return mcPlanet5Background;
    }

    public McPlanet5Background()
        : base("menu2/McPlanet5Background")
    {
    }

    public void Free()
    {
        StaticPool<McPlanet5Background>.Free(this);
    }
}
