using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFakeHeroHotspotBlack : Sprite, IFreeable, IId
{
    public const string ID = "fakeHero/McFakeHeroHotspotBlack";

    public string Id => "fakeHero/McFakeHeroHotspotBlack";

    public static McFakeHeroHotspotBlack New()
    {
        McFakeHeroHotspotBlack mcFakeHeroHotspotBlack = StaticPool<McFakeHeroHotspotBlack>.New();
        mcFakeHeroHotspotBlack.RefreshProperties();
        return mcFakeHeroHotspotBlack;
    }

    public McFakeHeroHotspotBlack()
        : base("fakeHero/McFakeHeroHotspotBlack")
    {
    }

    public void Free()
    {
        StaticPool<McFakeHeroHotspotBlack>.Free(this);
    }
}
