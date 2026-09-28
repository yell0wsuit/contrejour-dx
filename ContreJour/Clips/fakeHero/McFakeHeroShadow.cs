using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFakeHeroShadow : Sprite, IFreeable, IId
{
    public const string ID = "fakeHero/McFakeHeroShadow";

    public string Id => "fakeHero/McFakeHeroShadow";

    public static McFakeHeroShadow New()
    {
        McFakeHeroShadow mcFakeHeroShadow = StaticPool<McFakeHeroShadow>.New();
        mcFakeHeroShadow.RefreshProperties();
        return mcFakeHeroShadow;
    }

    public McFakeHeroShadow()
        : base("fakeHero/McFakeHeroShadow")
    {
    }

    public void Free()
    {
        StaticPool<McFakeHeroShadow>.Free(this);
    }
}
