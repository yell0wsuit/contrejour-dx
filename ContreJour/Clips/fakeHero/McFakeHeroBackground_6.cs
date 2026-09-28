using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFakeHeroBackground_6 : Sprite, IFreeable, IId
{
    public const string ID = "fakeHero/McFakeHeroBackground_6";

    public string Id => "fakeHero/McFakeHeroBackground_6";

    public static McFakeHeroBackground_6 New()
    {
        McFakeHeroBackground_6 mcFakeHeroBackground_ = StaticPool<McFakeHeroBackground_6>.New();
        mcFakeHeroBackground_.RefreshProperties();
        return mcFakeHeroBackground_;
    }

    public McFakeHeroBackground_6()
        : base("fakeHero/McFakeHeroBackground_6")
    {
    }

    public void Free()
    {
        StaticPool<McFakeHeroBackground_6>.Free(this);
    }
}
