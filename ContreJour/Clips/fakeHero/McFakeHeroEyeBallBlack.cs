using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFakeHeroEyeBallBlack : Sprite, IFreeable, IId
{
    public const string ID = "fakeHero/McFakeHeroEyeBallBlack";

    public string Id => "fakeHero/McFakeHeroEyeBallBlack";

    public static McFakeHeroEyeBallBlack New()
    {
        McFakeHeroEyeBallBlack mcFakeHeroEyeBallBlack = StaticPool<McFakeHeroEyeBallBlack>.New();
        mcFakeHeroEyeBallBlack.RefreshProperties();
        return mcFakeHeroEyeBallBlack;
    }

    public McFakeHeroEyeBallBlack()
        : base("fakeHero/McFakeHeroEyeBallBlack")
    {
    }

    public void Free()
    {
        StaticPool<McFakeHeroEyeBallBlack>.Free(this);
    }
}
