using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFakeHeroShadow_6 : Sprite, IFreeable, IId
{
    public const string ID = "fakeHero/McFakeHeroShadow_6";

    public string Id => "fakeHero/McFakeHeroShadow_6";

    public static McFakeHeroShadow_6 New()
    {
        McFakeHeroShadow_6 mcFakeHeroShadow_ = StaticPool<McFakeHeroShadow_6>.New();
        mcFakeHeroShadow_.RefreshProperties();
        return mcFakeHeroShadow_;
    }

    public McFakeHeroShadow_6()
        : base("fakeHero/McFakeHeroShadow_6")
    {
    }

    public void Free()
    {
        StaticPool<McFakeHeroShadow_6>.Free(this);
    }
}
