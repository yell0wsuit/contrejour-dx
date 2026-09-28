using System.CodeDom.Compiler;
using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.lights;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLightView1Content : Sprite, IFreeable, IId
{
    public const string ID = "lights/McLightView1Content";

    public string Id => "lights/McLightView1Content";

    public static McLightView1Content New()
    {
        McLightView1Content mcLightView1Content = StaticPool<McLightView1Content>.New();
        mcLightView1Content.RefreshProperties();
        return mcLightView1Content;
    }

    public McLightView1Content()
        : base("lights/McLightView1Content")
    {
    }

    public void Free()
    {
        StaticPool<McLightView1Content>.Free(this);
    }
}
