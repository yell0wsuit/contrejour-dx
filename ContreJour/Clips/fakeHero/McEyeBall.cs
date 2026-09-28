using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.fakeHero;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McEyeBall : Sprite, IFreeable, IId
{
    public const string ID = "fakeHero/McEyeBall";

    public string Id => "fakeHero/McEyeBall";

    public static McEyeBall New()
    {
        McEyeBall mcEyeBall = StaticPool<McEyeBall>.New();
        mcEyeBall.RefreshProperties();
        return mcEyeBall;
    }

    public McEyeBall()
        : base("fakeHero/McEyeBall")
    {
    }

    public void Free()
    {
        StaticPool<McEyeBall>.Free(this);
    }
}
