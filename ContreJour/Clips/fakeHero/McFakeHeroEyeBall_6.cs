using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFakeHeroEyeBall_6 : Sprite, IFreeable, IId
{
    public const string ID = "fakeHero/McFakeHeroEyeBall_6";

    public string Id => "fakeHero/McFakeHeroEyeBall_6";

    public static McFakeHeroEyeBall_6 New()
    {
        McFakeHeroEyeBall_6 mcFakeHeroEyeBall_ = StaticPool<McFakeHeroEyeBall_6>.New();
        mcFakeHeroEyeBall_.RefreshProperties();
        return mcFakeHeroEyeBall_;
    }

    public McFakeHeroEyeBall_6()
        : base("fakeHero/McFakeHeroEyeBall_6")
    {
    }

    public void Free()
    {
        StaticPool<McFakeHeroEyeBall_6>.Free(this);
    }
}
