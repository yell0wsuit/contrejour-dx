using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFakeHeroEyeSmile : MovieClip, IFreeable, IId
{
    public const string ID = "fakeHero/McFakeHeroEyeSmile";

    public string Id => "fakeHero/McFakeHeroEyeSmile";

    public static McFakeHeroEyeSmile New()
    {
        McFakeHeroEyeSmile mcFakeHeroEyeSmile = StaticPool<McFakeHeroEyeSmile>.New();
        mcFakeHeroEyeSmile.RefreshProperties();
        return mcFakeHeroEyeSmile;
    }

    public McFakeHeroEyeSmile()
        : base("fakeHero/McFakeHeroEyeSmile")
    {
    }

    public void Free()
    {
        StaticPool<McFakeHeroEyeSmile>.Free(this);
    }
}
