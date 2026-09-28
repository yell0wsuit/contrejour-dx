using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFakeHeroEyeBall : Sprite, IFreeable, IId
{
    public const string ID = "fakeHero/McFakeHeroEyeBall";

    public string Id => "fakeHero/McFakeHeroEyeBall";

    public static McFakeHeroEyeBall New()
    {
        McFakeHeroEyeBall mcFakeHeroEyeBall = StaticPool<McFakeHeroEyeBall>.New();
        mcFakeHeroEyeBall.RefreshProperties();
        return mcFakeHeroEyeBall;
    }

    public McFakeHeroEyeBall()
        : base("fakeHero/McFakeHeroEyeBall")
    {
    }

    public void Free()
    {
        StaticPool<McFakeHeroEyeBall>.Free(this);
    }
}
