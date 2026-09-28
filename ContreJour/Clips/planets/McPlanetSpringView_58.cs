using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.planets;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McPlanetSpringView_58 : Sprite, IFreeable, IId
{
    public const string ID = "planets/McPlanetSpringView_58";

    public string Id => "planets/McPlanetSpringView_58";

    public static McPlanetSpringView_58 New()
    {
        McPlanetSpringView_58 mcPlanetSpringView_ = StaticPool<McPlanetSpringView_58>.New();
        mcPlanetSpringView_.RefreshProperties();
        return mcPlanetSpringView_;
    }

    public McPlanetSpringView_58()
        : base("planets/McPlanetSpringView_58")
    {
    }

    public void Free()
    {
        StaticPool<McPlanetSpringView_58>.Free(this);
    }
}
