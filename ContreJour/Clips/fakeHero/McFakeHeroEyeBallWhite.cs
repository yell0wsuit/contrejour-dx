using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFakeHeroEyeBallWhite : Sprite, IFreeable, IId
{
    public const string ID = "fakeHero/McFakeHeroEyeBallWhite";

    public string Id => "fakeHero/McFakeHeroEyeBallWhite";

    public static McFakeHeroEyeBallWhite New()
    {
        McFakeHeroEyeBallWhite mcFakeHeroEyeBallWhite = StaticPool<McFakeHeroEyeBallWhite>.New();
        mcFakeHeroEyeBallWhite.RefreshProperties();
        return mcFakeHeroEyeBallWhite;
    }

    public McFakeHeroEyeBallWhite()
        : base("fakeHero/McFakeHeroEyeBallWhite")
    {
    }

    public void Free()
    {
        StaticPool<McFakeHeroEyeBallWhite>.Free(this);
    }
}
