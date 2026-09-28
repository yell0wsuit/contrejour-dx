using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McFakeHeroEye : Sprite, IFreeable, IId
{
    public const string ID = "fakeHero/McFakeHeroEye";

    public string Id => "fakeHero/McFakeHeroEye";

    public static McFakeHeroEye New()
    {
        McFakeHeroEye mcFakeHeroEye = StaticPool<McFakeHeroEye>.New();
        mcFakeHeroEye.RefreshProperties();
        return mcFakeHeroEye;
    }

    public McFakeHeroEye()
        : base("fakeHero/McFakeHeroEye")
    {
    }

    public void Free()
    {
        StaticPool<McFakeHeroEye>.Free(this);
    }
}
