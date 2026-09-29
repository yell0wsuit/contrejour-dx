using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFakeHeroEyeSmileBlack : MovieClip, IFreeable, IId
{
    public const string ID = "fakeHero/McFakeHeroEyeSmileBlack";

    public string Id => "fakeHero/McFakeHeroEyeSmileBlack";

    public static McFakeHeroEyeSmileBlack New()
    {
        McFakeHeroEyeSmileBlack mcFakeHeroEyeSmileBlack = StaticPool.New<McFakeHeroEyeSmileBlack>();
        mcFakeHeroEyeSmileBlack.RefreshProperties();
        return mcFakeHeroEyeSmileBlack;
    }

    public McFakeHeroEyeSmileBlack()
        : base("fakeHero/McFakeHeroEyeSmileBlack")
    {
    }

    public void Free()
    {
        StaticPool.Free<McFakeHeroEyeSmileBlack>(this);
    }
}
