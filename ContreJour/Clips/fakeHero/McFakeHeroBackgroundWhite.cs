using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFakeHeroBackgroundWhite : Sprite, IFreeable, IId
{
    public const string ID = "fakeHero/McFakeHeroBackgroundWhite";

    public string Id => "fakeHero/McFakeHeroBackgroundWhite";

    public static McFakeHeroBackgroundWhite New()
    {
        McFakeHeroBackgroundWhite mcFakeHeroBackgroundWhite = StaticPool<McFakeHeroBackgroundWhite>.New();
        mcFakeHeroBackgroundWhite.RefreshProperties();
        return mcFakeHeroBackgroundWhite;
    }

    public McFakeHeroBackgroundWhite()
        : base("fakeHero/McFakeHeroBackgroundWhite")
    {
    }

    public void Free()
    {
        StaticPool<McFakeHeroBackgroundWhite>.Free(this);
    }
}
