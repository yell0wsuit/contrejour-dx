using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFakeHeroShadowWhite : Sprite, IFreeable, IId
{
    public const string ID = "fakeHero/McFakeHeroShadowWhite";

    public string Id => "fakeHero/McFakeHeroShadowWhite";

    public static McFakeHeroShadowWhite New()
    {
        McFakeHeroShadowWhite mcFakeHeroShadowWhite = StaticPool<McFakeHeroShadowWhite>.New();
        mcFakeHeroShadowWhite.RefreshProperties();
        return mcFakeHeroShadowWhite;
    }

    public McFakeHeroShadowWhite()
        : base("fakeHero/McFakeHeroShadowWhite")
    {
    }

    public void Free()
    {
        StaticPool<McFakeHeroShadowWhite>.Free(this);
    }
}
