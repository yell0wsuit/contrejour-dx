using System.CodeDom.Compiler;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace ContreJour.Clips.lights;

[GeneratedCode("Mokus2D.ImagesExporter", "0.2")]
public class McLightView7Content : Sprite, IFreeable, IId
{
    public const string ID = "lights/McLightView7Content";

    public string Id => "lights/McLightView7Content";

    public static McLightView7Content New()
    {
        McLightView7Content mcLightView7Content = StaticPool<McLightView7Content>.New();
        mcLightView7Content.RefreshProperties();
        return mcLightView7Content;
    }

    public McLightView7Content()
        : base("lights/McLightView7Content")
    {
    }

    public void Free()
    {
        StaticPool<McLightView7Content>.Free(this);
    }
}
