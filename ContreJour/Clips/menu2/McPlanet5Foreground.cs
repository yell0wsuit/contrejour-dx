using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanet5Foreground : Sprite, IFreeable, IId
{
    public const string ID = "menu2/McPlanet5Foreground";

    public string Id => "menu2/McPlanet5Foreground";

    public static McPlanet5Foreground New()
    {
        McPlanet5Foreground mcPlanet5Foreground = StaticPool<McPlanet5Foreground>.New();
        mcPlanet5Foreground.RefreshProperties();
        return mcPlanet5Foreground;
    }

    public McPlanet5Foreground()
        : base("menu2/McPlanet5Foreground")
    {
    }

    public void Free()
    {
        StaticPool<McPlanet5Foreground>.Free(this);
    }
}
