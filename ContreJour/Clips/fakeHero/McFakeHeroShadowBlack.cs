using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFakeHeroShadowBlack : Sprite, IFreeable, IId
{
    public const string ID = "fakeHero/McFakeHeroShadowBlack";

    public string Id => "fakeHero/McFakeHeroShadowBlack";

    public static McFakeHeroShadowBlack New()
    {
        McFakeHeroShadowBlack mcFakeHeroShadowBlack = StaticPool<McFakeHeroShadowBlack>.New();
        mcFakeHeroShadowBlack.RefreshProperties();
        return mcFakeHeroShadowBlack;
    }

    public McFakeHeroShadowBlack()
        : base("fakeHero/McFakeHeroShadowBlack")
    {
    }

    public void Free()
    {
        StaticPool<McFakeHeroShadowBlack>.Free(this);
    }
}
