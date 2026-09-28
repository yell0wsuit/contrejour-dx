using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFakeHeroBackgroundBlack : Sprite, IFreeable, IId
{
    public const string ID = "fakeHero/McFakeHeroBackgroundBlack";

    public string Id => "fakeHero/McFakeHeroBackgroundBlack";

    public static McFakeHeroBackgroundBlack New()
    {
        McFakeHeroBackgroundBlack mcFakeHeroBackgroundBlack = StaticPool<McFakeHeroBackgroundBlack>.New();
        mcFakeHeroBackgroundBlack.RefreshProperties();
        return mcFakeHeroBackgroundBlack;
    }

    public McFakeHeroBackgroundBlack()
        : base("fakeHero/McFakeHeroBackgroundBlack")
    {
    }

    public void Free()
    {
        StaticPool<McFakeHeroBackgroundBlack>.Free(this);
    }
}
