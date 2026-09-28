using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFakeHeroEyeBlack : Sprite, IFreeable, IId
{
    public const string ID = "fakeHero/McFakeHeroEyeBlack";

    public string Id => "fakeHero/McFakeHeroEyeBlack";

    public static McFakeHeroEyeBlack New()
    {
        McFakeHeroEyeBlack mcFakeHeroEyeBlack = StaticPool<McFakeHeroEyeBlack>.New();
        mcFakeHeroEyeBlack.RefreshProperties();
        return mcFakeHeroEyeBlack;
    }

    public McFakeHeroEyeBlack()
        : base("fakeHero/McFakeHeroEyeBlack")
    {
    }

    public void Free()
    {
        StaticPool<McFakeHeroEyeBlack>.Free(this);
    }
}
