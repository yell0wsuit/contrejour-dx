using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFakeHeroHotspot : Sprite, IFreeable, IId
{
    public const string ID = "fakeHero/McFakeHeroHotspot";

    public string Id => "fakeHero/McFakeHeroHotspot";

    public static McFakeHeroHotspot New()
    {
        McFakeHeroHotspot mcFakeHeroHotspot = StaticPool<McFakeHeroHotspot>.New();
        mcFakeHeroHotspot.RefreshProperties();
        return mcFakeHeroHotspot;
    }

    public McFakeHeroHotspot()
        : base("fakeHero/McFakeHeroHotspot")
    {
    }

    public void Free()
    {
        StaticPool<McFakeHeroHotspot>.Free(this);
    }
}
