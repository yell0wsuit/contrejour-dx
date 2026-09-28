using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.menu2;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanet4Background : Sprite, IFreeable, IId
{
    public const string ID = "menu2/McPlanet4Background";

    public string Id => "menu2/McPlanet4Background";

    public static McPlanet4Background New()
    {
        McPlanet4Background mcPlanet4Background = StaticPool<McPlanet4Background>.New();
        mcPlanet4Background.RefreshProperties();
        return mcPlanet4Background;
    }

    public McPlanet4Background()
        : base("menu2/McPlanet4Background")
    {
    }

    public void Free()
    {
        StaticPool<McPlanet4Background>.Free(this);
    }
}
