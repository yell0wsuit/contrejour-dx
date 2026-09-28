using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanet4Spring0 : Sprite, IFreeable, IId
{
    public const string ID = "menu2/McPlanet4Spring0";

    public string Id => "menu2/McPlanet4Spring0";

    public static McPlanet4Spring0 New()
    {
        McPlanet4Spring0 mcPlanet4Spring = StaticPool<McPlanet4Spring0>.New();
        mcPlanet4Spring.RefreshProperties();
        return mcPlanet4Spring;
    }

    public McPlanet4Spring0()
        : base("menu2/McPlanet4Spring0")
    {
    }

    public void Free()
    {
        StaticPool<McPlanet4Spring0>.Free(this);
    }
}
